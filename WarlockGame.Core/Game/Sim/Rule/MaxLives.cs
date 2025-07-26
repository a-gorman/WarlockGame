using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Rule;

class MaxLives {
    public int InitialLives { get; }
    private readonly Simulation _simulation;
    public Dictionary<int, int> PlayerLives { get; } = new();
    public event Action<MaxLivesChanged>? OnChanged;

    public MaxLives(Simulation simulation, int initialLives) {
        _simulation = simulation;
        InitialLives = initialLives;
    }
    
    public void Reset() {
        PlayerLives.Clear();
        foreach (var player in PlayerManager.Players) {
            PlayerLives.Add(player.Id, InitialLives);
        }

        OnChanged?.Invoke(new MaxLivesChanged { Reset = true });
    }

    public void OnWarlockDestroyed(Warlock warlock) {
        int playerId = warlock.PlayerId!.Value;
        PlayerLives[playerId] -= 1;
        OnChanged?.Invoke(new MaxLivesChanged { PlayerId = playerId });

        if (PlayerLives[playerId] != 0) {
            _simulation.EffectManager.AddDelayedEffect(() => {
                var respawnPosition = Simulation.ArenaSize / 2 + new Vector2(250, 0).Rotated(_simulation.Random.NextAngle());
                _simulation.EntityManager.Add(_simulation.WarlockFactory.CreateWarlock(playerId, respawnPosition));
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

public struct MaxLivesChanged {
    public bool Reset { get; set; }
    public int PlayerId { get; set; }
}