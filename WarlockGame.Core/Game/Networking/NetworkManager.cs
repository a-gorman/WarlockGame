using System;
using System.Data;
using LiteNetLib;
using LiteNetLib.Utils;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.UI;

namespace WarlockGame.Core.Game.Networking; 

static class NetworkManager {
    private static Client? _client;
    private static Server? _server;

    public static bool IsConnected => IsClient || IsServer;
    public static bool IsClient => _client != null;
    public static bool IsServer => _server != null;

    /// <summary>
    /// Latency in milliseconds
    /// </summary>
    private static int? Latency => IsClient? _client!.Latency : _server?.Latency;
    
    public static void StartServer() {
        if (IsConnected) return;
        
        _server = new Server();
        _server.Start();
        MessageDisplay.Display("Game hosted.");
    }

    public static void ConnectToServer(string address, Action clientConnectedCallback) {
        if (IsConnected) return;
        
        _client = new Client();
        _client.Connect(address, clientConnectedCallback);
    }

    public static void Update() {
        if (_server != null) {
            _server.Update();
        }

        if (_client != null) {
            _client.Update();
        }
        
        // SimDebug.Visualize($"Latency: {Latency}", new Vector2(800, 50));
    }
    
    public static void JoinGame(string playerName) {
        if(!IsClient) throw new ConstraintException("Can't get join game when not connected to server");

        Logger.Info("Joining game");
        MessageDisplay.Display("Joined new game");
        
        _client!.SendSerializable(new JoinGameRequest { PlayerName = playerName }, DeliveryMethod.ReliableOrdered);
    }

    public static void Disconnect() {
        _server = null;
        _client = null;
    }

    public static void SendSerializable<T>(T packet) where T : INetSerializable {
        if (IsServer) {
            _server!.SendSerializableToAll(packet, DeliveryMethod.ReliableOrdered);
        }
        else {
            _client!.SendSerializable(packet, DeliveryMethod.ReliableOrdered);
        }
    }
    
    public static void Send<T>(T packet) where T : class, new() {
        if (IsServer) {
            _server!.SendToAll(packet, DeliveryMethod.ReliableOrdered);
        }
        else {
            _client!.Send(packet, DeliveryMethod.ReliableOrdered);
        }
    }
}