using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Effect;
using WarlockGame.Core.Game.Entity;

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
                var respawnPosition = Sim.Simulation.ArenaSize / 2 + new Vector2(250, 0).Rotate(Sim.Simulation.Instance.Random.NextAngle());
                EntityManager.Add(new Warlock(playerId) { Position = respawnPosition });
            }, GameTimer.FromSeconds(5));
        }
    }
}