using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Input;

namespace WarlockGame.Core.Game; 

static class PlayerManager {
    public static List<Player> Players { get; } = new();

    public static Player? LocalPlayer { get; private set; }

    public static Player AddLocalPlayer(string name) {
        if (LocalPlayer is not null) throw new InvalidOperationException("Local player already exists");
        
        var player = CreatePlayer(name, true);
        InputManager.AttachLocalGameInput(player);
        LocalPlayer = player;
        return player;
    }
    
    public static Player AddRemotePlayer(string name) {
        return CreatePlayer(name, false);
    }

    private static Player CreatePlayer(string name, bool isLocal) {
        var id = Players.Select(x => x.Id).DefaultIfEmpty().Max() + 1;
        
        var player = new Player(name, id, isLocal);
        
        Players.Add(player);

        return player;
    }

    public static Player? GetPlayer(int playerId) {
        return Players.Find(x => x.Id == playerId);
    }

    public static void RemovePlayer(int playerId) {
        var playerToRemove = Players.FirstOrDefault(x => x.Id == playerId);
        if(playerToRemove == null || playerToRemove.IsLocal) { return; }

        Players.Remove(playerToRemove);
        EntityManager.GetWarlockByPlayerId(playerToRemove.Id)!.IsExpired = true;
    }
}