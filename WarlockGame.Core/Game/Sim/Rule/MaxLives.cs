using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Rule;

class MaxLives {
    public int InitialLives { get; private set; }
    private readonly Simulation _simulation;
    public Dictionary<int, int> PlayerLives { get; } = new();

    public MaxLives(Simulation simulation, int initialLives) {
        _simulation = simulation;
        InitialLives = initialLives;
    }
    
    public void Reset() {
        PlayerLives.Clear();
        foreach (var player in PlayerManager.Players) {
            PlayerLives.Add(player.Id, InitialLives);
        }
    }

    public void OnWarlockDestroyed(Warlock warlock) {
        int playerId = warlock.PlayerId;
        PlayerLives[playerId] -= 1;

        if (PlayerLives[playerId] != 0) {
            _simulation.EffectManager.AddDelayedEffect(() => {
                var respawnPosition = Simulation.ArenaSize / 2 + new Vector2(250, 0).Rotated(_simulation.Random.NextAngle());
                _simulation.EntityManager.Add(new Warlock(playerId, respawnPosition, _simulation));
            }, SimTime.OfSeconds(5));
        }
        
        if (PlayerLives.Count(x => x.Value > 0) == 1) {
            var winningPlayerId = PlayerLives.First(x => x.Value > 0).Key;
            var message = winningPlayerId == PlayerManager.LocalPlayer!.Id
                ? "You Win!!"
                : $"{ PlayerManager.GetPlayer(winningPlayerId)?.Name ?? "Another player" } has won the game!";
            SimDebug.Visualize(message, Simulation.ArenaSize / 2, 500);
        }
        else if (PlayerLives.All(x => x.Value == 0)) {
            SimDebug.Visualize("It's a draw!", Simulation.ArenaSize / 2, 500);
        }
    }
}