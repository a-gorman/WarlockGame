using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Entity.Order;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.UI;
using Warlock = WarlockGame.Core.Game.Sim.Entity.Warlock;

namespace WarlockGame.Core.Game.Sim;

class Simulation {
    public int Tick { get; set; }

    public static Simulation Instance { get; private set; }  = new Simulation();
    
    private readonly MaxLives _gameRule = new() { InitialLives = 2 };
    public Random Random { get; private set; } = new Random();
    public int Checksum { get; private set; }
    public static Vector2 ArenaSize => new Vector2(1900, 1000);

    public void Initialize() {
        UIManager.AddComponent(new SpellDisplay());
        UIManager.AddComponent(new HealthBarManager());
        
        EntityManager.WarlockDestroyed += _gameRule.OnWarlockDestroyed;
    }

    public void Update(IEnumerable<IPlayerCommand> inputs) {
        Tick++;

        // Debug.Visualize($"Frame: {Tick}", new Vector2(1500, 0));

        foreach (var command in inputs) {
            IssuePlayerCommand(command);
        }
        
        EntityManager.Update();
        EffectManager.Update();

        Checksum = CalculateChecksum();
    }
    
    public void Restart(int seed) {
        ClearGameState();
        Random = new Random(seed);
        var radiansPerPlayer = (float)(2 * Math.PI / PlayerManager.Players.Count);
        var warlocks = PlayerManager.Players.Select((x, i) => new Warlock(x.Id)
            { Position = ArenaSize / 2 + new Vector2(0, 250).Rotate(radiansPerPlayer * i) });
        foreach (var warlock in warlocks) {
            Logger.Info($"Creating warlock at: {warlock.Position / 2}");
            EntityManager.Add(warlock);
        }

        Checksum = CalculateChecksum();

        _gameRule.Reset();
    }
    
    private void ClearGameState() {
        Tick = 0;
        EntityManager.Clear();
        EffectManager.Clear();
    }
    
    private static int CalculateChecksum() {
        return (int)EntityManager.Warlocks.Sum(x => x.Position.X + x.Position.Y);
    }
    
    private static void IssuePlayerCommand(IPlayerCommand action) {
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
}