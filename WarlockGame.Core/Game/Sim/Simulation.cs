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
using WarlockGame.Core.Game.UI.Components;
using WarlockGame.Core.Game.Util;
using Warlock = WarlockGame.Core.Game.Sim.Entities.Warlock;

namespace WarlockGame.Core.Game.Sim;

class Simulation {
    private DamagingGround? _damagingGround;
    public int Tick { get; private set; }

    public Random Random { get; private set; } = new();
    public EntityManager EntityManager { get; } = new();
    public SpellManager SpellManager { get; }
    public SpellFactory SpellFactory { get; }
    public EffectManager EffectManager { get; }
    public PerkManager PerkManager { get; }
    public EventManager EventManager { get; } = new();

    public GameRules GameRules { get; }

    public Force[] Forces { get; private set; } = [];

    public static Vector2 ArenaSize { 
        get;
        private set {
            ArenaCenter = value / 2;
            field = value;
        }
    }
    
    public static Vector2 ArenaCenter { get; private set; }

    public Simulation() {
        GameRules = new GameRules(this, 4);
        EffectManager = new EffectManager();
        SpellFactory = new SpellFactory(this);
        SpellManager = new SpellManager(SpellFactory);
        PerkManager = new PerkManager(this);

        ArenaSize = GameRules.InitialArenaSize;
    }

    public void Initialize() {
        GameRules.Initialize();
        PerkManager.Initialize();
        EntityManager.WarlockDestroyed += GameRules.OnWarlockDestroyed;

        Logger.Info("Simulation initialized", Logger.LogType.Program);
    }

    public TickResult Update(IEnumerable<IPlayerAction> inputs) {
        Tick++;

        _damagingGround?.Shape = _damagingGround.Shape with { Radius = Math.Max(_damagingGround.Shape.Radius - 0.1f, 300) };

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
    
    public void Restart(int seed, int forces) {
        ClearGameState();
        Random = new Random(seed);
        Forces = Enumerable.Range(1, forces).Select(x => new Force { Id = x }).ToArray();
        GameRules.Reset();
        
        foreach (var force in Forces) {
            foreach (var spellType in GameRules.StartingSpells) {
                SpellManager.AddSpell(force.Id, spellType);
            }
        }
        
        EventManager.OnSimRestarted();
    }

    private void ClearGameState() {
        Tick = 0;
        PerkManager.Clear();
        SpellManager.Clear();
        EntityManager.Clear();
        EffectManager.Clear();
    }

    public int CalculateChecksum() {
        return (int)EntityManager.EntitiesLivingOrDead.Sum(x => x.Position.X + x.Position.Y);
    }

    private void ProcessPlayerAction(IPlayerAction action) {
        Logger.Debug($"Issuing {action.GetSerializerType()} command for player {action.PlayerId}", Logger.LogType.PlayerAction | Logger.LogType.Simulation);
        
        // Violation: Simulation reaching outside simulation
        var player = PlayerManager.GetPlayer(action.PlayerId);
        if (player?.ForceId == null) {
            Logger.Warning("Tried to process player command for player without a force that does not exist! Skipping...", Logger.LogType.PlayerAction | Logger.LogType.Simulation);
            return;
        }

        var forceId = player.ForceId.Value;
        
        switch (action) {
            case MoveAction move:
                EntityManager.GetWarlockByForceId(forceId)
                    ?.GiveOrder(x => new DestinationMoveOrder(move.Location, x));
                break;
            case CastAction cast:
                EntityManager.GetWarlockByForceId(forceId)
                    ?.GiveOrder(x => new CastOrder(cast.SpellId, cast.CastVector, x, cast.Type.ToSimType()));
                break;
            case SelectPerk selectPerk:
                
                var perk = PerkManager.ChoosePerk(forceId, selectPerk.PerkId);
                var playerName = player.Name;
                MessageDisplay.Display($"{playerName} picked the {perk?.Name} perk!");
                break;
            case SelectSpells selectSpells:
                var force = Forces.FirstOrDefault(x => x.Id == forceId);

                if (force == null) {
                    Logger.Error($"Tried choosing spells for invalid force! {selectSpells.PlayerId}", Logger.LogType.Simulation | Logger.LogType.PlayerAction);
                    break;
                }
                
                if (force.AreSpellsChosen) {
                    Logger.Error($"Spells have been chosen more than once! {selectSpells.PlayerId}", Logger.LogType.Simulation | Logger.LogType.PlayerAction);
                    break;
                }
                
                foreach (var spell in selectSpells.SpellDefIds) {
                    SpellManager.AddSpell(force.Id, spell);
                    Logger.Info($"Spell chosen! {force.Id} chose {spell}", Logger.LogType.Simulation | Logger.LogType.PlayerAction);
                }

                force.AreSpellsChosen = true;
                
                if (Forces.All(x => x.AreSpellsChosen)) {
                    StartRound();
                }
                break;
        }
    }

    private void StartRound() {
        var radiansPerPlayer = (float)(2 * Math.PI / Forces.Length);
        var warlocks = Forces.Select((x, i) => {
            var spawnPos = ArenaSize / 2 + new Vector2(0, 400).Rotated(radiansPerPlayer * i);
            var warlock = new Warlock(x.Id, spawnPos, this);

            return warlock;
        });
        foreach (var warlock in warlocks) {
            Logger.Info($"Creating warlock at: {warlock.Position}", Logger.LogType.Simulation);
            EntityManager.Add(warlock);
        }
        
        _damagingGround = new DamagingGround(this, new CircleF(ArenaCenter, (ArenaSize / 2).Length()), 0.1f, inverted: true);
        EffectManager.Add(_damagingGround);

        SimDebug.Visualize(new Rectangle(Vector2.Zero.ToPoint(), ArenaSize.ToPoint()), Color.MonoGameOrange, int.MaxValue);
    }

    public ref struct TickResult {
        public int Tick { get; init; }
        public int Checksum { get; init; }
    }

    public class Force {
        public required int Id { get; init; }
        public bool AreSpellsChosen { get; set; }
    }
}