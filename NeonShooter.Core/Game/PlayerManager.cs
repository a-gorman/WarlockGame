using System;
using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game.Entity;

namespace NeonShooter.Core.Game; 

static class PlayerManager {
    public static List<Player> Players { get; } = new();

    public static Player ActivePlayer => Players.First();

    public static void AddPlayer(string name) {
        var player = new Player {Name = name};
        var playerShip = new PlayerShip(player);
        player.Warlock = playerShip;

        Players.Add(player);
        EntityManager.Add(playerShip);
    }

    public static void Update() {
        foreach (var player in Players) {
            player.Update();
        }
    }
}