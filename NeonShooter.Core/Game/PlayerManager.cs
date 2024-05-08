using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.UX;

namespace NeonShooter.Core.Game; 

static class PlayerManager {
    public static List<Player> Players { get; } = new();

    public static Player ActivePlayer => Players.First();

    public static void AddLocalPlayer(string name, int id, InputManager.DeviceType deviceType) {
        var warlock = new Warlock(id, id);
        var player = new Player(name, id, warlock);
        
        Players.Add(player);
        EntityManager.Add(warlock);
        InputManager.AttachLocalInput(player, deviceType);
    }

    public static void AddRemotePlayer(Player player) {
        Players.Add(player);
        EntityManager.Add(player.Warlock);
    }

    public static Player? GetPlayer(int playerId) {
        return Players.Find(x => x.Id == playerId);
    }

    public static void Update() {
        foreach (var player in Players) {
            player.Update();
        }
    }
}