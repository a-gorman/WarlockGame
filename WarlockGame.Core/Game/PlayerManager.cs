using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game; 

static class PlayerManager {
    public static List<Player> Players { get; } = new();
    
    public static Player? LocalPlayer { get; private set; }
    public static int? LocalPlayerId { get; private set; }
    private static readonly Color[] PlayerColors = [Color.White, Color.Blue, Color.Red, Color.Yellow, Color.Green, Color.Purple, Color.Orange, Color.Brown, Color.Gold, Color.Gray];

    public static Player AddLocalPlayer(string name, Color? color, int? id = null) {
        if (LocalPlayer is not null) throw new InvalidOperationException("Local player already exists");
        id ??= Players.Select(x => x.Id).DefaultIfEmpty().Max() + 1;
        
        var player = CreatePlayer(id.Value, name, color ?? GetColor(id.Value), true);
        LocalPlayer = player;
        LocalPlayerId = id.Value;
        return player;
    }

    public static Player AddRemotePlayer(string name, Color? color, int? id = null) {
        id ??= GetNextPlayerId();

        return CreatePlayer(id.Value, name, color ?? GetColor(id.Value), false);
    }

    private static Player CreatePlayer(int id, string name, Color color, bool isLocal) {
        // We won't have a forceId until the game is restarted
        var player = new Player(name, id, color, isLocal, forceId: null);

        Players.Add(player);

        return player;
    }

    // This method should not be used when connecting to simulation objects
    public static Player? GetPlayer(int playerId) {
        return Players.Find(x => x.Id == playerId);
    }
    
    public static Player? GetPlayerByForceId(int forceId) {
        return Players.Find(x => x.ForceId == forceId);
    }

    private static Color GetColor(int id) {
        if (id < PlayerColors.Length - 1) {
            return PlayerColors[id];
        }

        return new Color(new Vector3(Random.Shared.Next(), Random.Shared.Next(), Random.Shared.Next()));
    }

    private static int GetNextPlayerId() {
        return Players.Select(x => x.Id).DefaultIfEmpty().Max() + 1;
    }

    public static bool IsLocal(int playerId) {
        return LocalPlayer != null && LocalPlayer.Id == playerId;
    }

    public static void OnGameRestarted() {
        int forceId = 1;
        foreach (var player in Players) {
            if (!player.IsActive) {
                player.ForceId = null;
            } else {
                player.ForceId = forceId;
                forceId++;
            }
        }
    }
}