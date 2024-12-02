using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Effect;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim;

class Simulation {
    public int Tick { get; set; }

    public static Simulation Instance { get; private set; }  = new Simulation();
    
    private readonly MaxLives _gameRule = new() { InitialLives = 3 };
    public Random Random { get; private set; } = new Random();
    public static Vector2 ArenaSize => new Vector2(1900, 1000);

    public void Initialize() {
        UIManager.AddComponent(new SpellDisplay());
        UIManager.AddComponent(new HealthBarManager());
        
        EntityManager.WarlockDestroyed += _gameRule.OnWarlockDestroyed;
    }

    public void Update() {
        Tick++;

        Debug.Visualize($"Frame: {Tick}", new Vector2(1500, 0));
        
        CommandProcessor.Update(Tick);
        EntityManager.Update();
        EffectManager.Update();
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

        _gameRule.Reset();
    }
    
    private void ClearGameState() {
        // Tick = 0;
        CommandProcessor.Clear();
        EntityManager.Clear();
        EffectManager.Clear();
    }
}