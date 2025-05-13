using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Order;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.Util;
using Warlock = WarlockGame.Core.Game.Sim.Entities.Warlock;

namespace WarlockGame.Core.Game.Sim;

class Simulation {
    public int Tick { get; set; }

    public Random Random { get; private set; } = new();
    public EntityManager EntityManager { get; } = new();
    public SpellFactory SpellFactory { get; }
    public EffectManager EffectManager { get; }

    private readonly MaxLives _gameRule;

    public static Vector2 ArenaSize => new Vector2(1900, 1000);

    public Simulation() {
        EffectManager = new EffectManager();
        SpellFactory = new SpellFactory(this);
        _gameRule = new MaxLives(this, 3);
        EntityManager.WarlockDestroyed += _gameRule.OnWarlockDestroyed;
    }

    public TickResult Update(IEnumerable<IPlayerCommand> inputs) {
        Tick++;

        foreach (var command in inputs) {
            IssuePlayerCommand(command);
        }
        
        EntityManager.Update();
        EffectManager.Update();

        return new TickResult
        {
            Tick = Tick,
            Checksum = CalculateChecksum()
        };
    }
    
    public void Restart(int seed) {
        ClearGameState();
        Random = new Random(seed);
        var radiansPerPlayer = (float)(2 * Math.PI / PlayerManager.Players.Count);
        var warlocks = PlayerManager.Players.Select((x, i) => {
            var spawnPos = ArenaSize / 2 + new Vector2(0, 250).Rotated(radiansPerPlayer * i);
            return new Warlock(x.Id, spawnPos, this);
        });
        foreach (var warlock in warlocks) {
            Logger.Info($"Creating warlock at: {warlock.Position}");
            EntityManager.Add(warlock);
        }
        _gameRule.Reset();
    }
    
    private void ClearGameState() {
        Tick = 0;
        EntityManager.Clear();
        EffectManager.Clear();
    }
    
    public int CalculateChecksum() {
        return (int)EntityManager.Warlocks.Sum(x => x.Position.X + x.Position.Y);
    }
    
    private void IssuePlayerCommand(IPlayerCommand action) {
        Logger.Debug($"Issuing {action.GetSerializerType()} command for player {action.PlayerId}");
        switch (action) {
            case MoveCommand move:
                EntityManager.GetWarlockByPlayerId(move.PlayerId)
                             ?.GiveOrder(x => new DestinationMoveOrder(move.Location, x));
                break;
            case CastCommand cast:
                EntityManager.GetWarlockByPlayerId(cast.PlayerId)
                             ?.GiveOrder(x => new CastOrder(cast.SpellId, cast.CastVector, x));
                break;
        }
    }

    public ref struct TickResult {
        public int Tick { get; init; }
        public int Checksum { get; init; }
    }
}