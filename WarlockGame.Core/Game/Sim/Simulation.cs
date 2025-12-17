using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Order;
using WarlockGame.Core.Game.Sim.Perks;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.UI.Components;
using WarlockGame.Core.Game.Util;
using Warlock = WarlockGame.Core.Game.Sim.Entities.Warlock;

namespace WarlockGame.Core.Game.Sim;

class Simulation {
    private DamagingGround _damagingGround = null!;
    public int Tick { get; private set; }

    public Random Random { get; private set; } = new();
    public EntityManager EntityManager { get; } = new();
    public SpellManager SpellManager { get; }
    public SpellFactory SpellFactory { get; }
    public EffectManager EffectManager { get; }
    public PerkManager PerkManager { get; }

    public GameRules GameRules { get; }

    public static Vector2 ArenaSize { 
        get;
        private set {
            ArenaCenter = value / 2;
            field = value;
        }
    }
    public static Vector2 ArenaCenter { get; private set; }

    public Simulation() {
        GameRules = new GameRules(this, 3);
        EffectManager = new EffectManager();
        SpellFactory = new SpellFactory(this);
        SpellManager = new SpellManager(SpellFactory);
        PerkManager = new PerkManager(this);

        ArenaSize = GameRules.InitialArenaSize;
        
        EntityManager.WarlockDestroyed += GameRules.OnWarlockDestroyed;
    }

    public void Initialize() {
        GameRules.Initialize();
        PerkManager.Initialize();
    }

    public TickResult Update(IEnumerable<IPlayerAction> inputs) {
        Tick++;

        _damagingGround.Shape = _damagingGround.Shape with { Radius = _damagingGround.Shape.Radius - 0.1f };

        foreach (var command in inputs) {
            ProcessPlayerAction(command);
        }
        
        EntityManager.Update();
        EffectManager.Update();
        SpellManager.Update();
        PerkManager.Update();
        
        return new TickResult
        {
            Tick = Tick,
            Checksum = CalculateChecksum()
        };
    }
    
    public void Restart(int seed) {
        ClearGameState();
        Random = new Random(seed);
        GameRules.Reset();
        UIManager.AddComponent(new Scoreboard(GameRules));
        
        var radiansPerPlayer = (float)(2 * Math.PI / PlayerManager.Players.Count);
        var warlocks = PlayerManager.Players.Select((x, i) => {
            var spawnPos = ArenaSize / 2 + new Vector2(0, 400).Rotated(radiansPerPlayer * i);
            var warlock = new Warlock(x.Id, spawnPos, this);

            warlock.Sprite.Color = PlayerManager.GetPlayer(x.Id)!.Color;
        
            return warlock;
        });
        foreach (var warlock in warlocks) {
            Logger.Info($"Creating warlock at: {warlock.Position}", Logger.LogType.Simulation);
            EntityManager.Add(warlock);
            foreach (var spellType in GameRules.StartingSpells) {
                SpellManager.AddSpell(warlock.PlayerId!.Value, spellType);
            }
        }
        
        _damagingGround = new DamagingGround(this, new CircleF(ArenaCenter, (ArenaSize / 2).Length()), 0.1f, inverted: true);
        EffectManager.Add(_damagingGround);

        SimDebug.Visualize(new Rectangle(Vector2.Zero.ToPoint(), ArenaSize.ToPoint()), Color.MonoGameOrange, int.MaxValue);
    }

    private void ClearGameState() {
        Tick = 0;
        EntityManager.Clear();
        EffectManager.Clear();
        SpellManager.Clear();
    }
    
    public int CalculateChecksum() {
        return (int)EntityManager.EntitiesLivingOrDead.Sum(x => x.Position.X + x.Position.Y);
    }
    
    private void ProcessPlayerAction(IPlayerAction action) {
        Logger.Debug($"Issuing {action.GetSerializerType()} command for player {action.PlayerId}", Logger.LogType.PlayerAction | Logger.LogType.Simulation);
        switch (action) {
            case MoveAction move:
                EntityManager.GetWarlockByForceId(move.PlayerId)
                             ?.GiveOrder(x => new DestinationMoveOrder(move.Location, x));
                break;
            case CastAction cast:
                EntityManager.GetWarlockByForceId(cast.PlayerId)
                             ?.GiveOrder(x => new CastOrder(cast.SpellId, cast.CastVector, x, cast.Type.ToSimType()));
                break;
            case SelectPerk selectPerk:
                PerkManager.ChoosePerk(selectPerk.PlayerId, selectPerk.PerkType);
                break;
        }
    }

    public ref struct TickResult {
        public int Tick { get; init; }
        public int Checksum { get; init; }
    }
}