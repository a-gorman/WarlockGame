using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Effect;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Log;
using Debug = WarlockGame.Core.Game.Util.Debug;

namespace WarlockGame.Core.Game.Sim.Rule;

class MaxLives {
    public required int InitialLives { get; init; }
    public Dictionary<int, int> PlayerLives { get; } = new();

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
            EffectManager.AddDelayedEffect(() => {
                var respawnPosition = Simulation.ArenaSize / 2 + new Vector2(250, 0).Rotate(Simulation.Instance.Random.NextAngle());
                EntityManager.Add(new Warlock(playerId) { Position = respawnPosition });
            }, GameTimer.FromSeconds(5));
        }
        
        if (PlayerLives.Count(x => x.Value > 0) == 1) {
            var message = PlayerLives.First(x => x.Value > 0).Key == PlayerManager.LocalPlayer.Id
                ? "You Win!!"
                : "Another player has won the game!";
            Debug.Visualize(message, Simulation.ArenaSize / 2, 100);
        }
        else if (PlayerLives.All(x => x.Value == 0)) {
            Debug.Visualize("It's a draw!", Simulation.ArenaSize / 2, 100);
        }
    }
}