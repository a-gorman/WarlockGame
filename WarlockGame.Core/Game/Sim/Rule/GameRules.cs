using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Perks;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Rule;

class GameRules {
    public Vector2 InitialArenaSize = new Vector2(2500);
    public int InitialLives { get; }
    
    public Dictionary<int, PlayerStatus> Statuses { get; } = new();

    public const int SpellSelections = 4;
    
    public readonly int[] StartingSpells = [1, 4];
    
    public readonly int[] AvailableSpells = [1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12];
    
    public event Action<LivesChanged>? OnChanged;

    private readonly Simulation _sim;
    
    public GameRules(Simulation sim, int initialLives) {
        _sim = sim;
        InitialLives = initialLives;
    }

    public void Initialize() {
        _sim.PerkManager.PerkChosen += OnPerkChosen;
    }
    
    public void Reset() {
        Statuses.Clear();
        foreach (var force in _sim.Forces) {
            Statuses.Add(force.Id, new PlayerStatus(InitialLives));
        }

        OnChanged?.Invoke(new LivesChanged { Reset = true });
    }

    public void OnWarlockDestroyed(Warlock warlock) {
        int forceId = warlock.ForceId!.Value;
        var status = Statuses[forceId];
        status.Lives -= 1;
        OnChanged?.Invoke(new LivesChanged { PlayerId = forceId });

        if (status.Lives != 0) {
            status.ChoosingPerk = true;
        }
        
        if (Statuses.Count(x => x.Value.Lives > 0) == 1) {
            var winningPlayerId = Statuses.First(x => x.Value.Lives > 0).Key;
            var message = winningPlayerId == PlayerManager.LocalPlayer!.Id
                ? "You Win!!"
                : $"{ PlayerManager.GetPlayer(winningPlayerId)?.Name ?? "Another player" } has won the game!";
            SimDebug.Visualize(message, Simulation.ArenaSize / 2, 500);
        }
        else if (Statuses.All(x => x.Value.Lives == 0)) {
            SimDebug.Visualize("It's a draw!", Simulation.ArenaSize / 2, 500);
        }
    }

    private void OnPerkChosen(int forceId, Perk _) {
        if (!Statuses[forceId].ChoosingPerk) return;
        
        Statuses[forceId].ChoosingPerk = false;
        _sim.EffectManager.AddDelayedEffect(() => {
            var respawnPosition = Simulation.ArenaSize / 2 + new Vector2(400, 0).Rotated(_sim.Random.NextAngle());
            _sim.EntityManager.RespawnWarlock(forceId, respawnPosition);
        }, SimTime.OfSeconds(1));
    }
}