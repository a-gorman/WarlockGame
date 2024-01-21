using System.Data;
using LiteNetLib;
using NeonShooter.Core.Game.Log;

namespace NeonShooter.Core.Game.Networking; 

static class NetworkManager {

    private static Client? _client;
    private static Server? _server;

    private static uint _frame;
    public static bool IsConnected => IsClient || IsServer;
    public static bool IsClient => _client != null;
    public static bool IsServer => _server != null;

    public static void StartServer() {
        if (IsConnected) return;
        
        _server = new Server();
        _server.Start();
    }

    public static void ConnectToServer() {
        if (IsConnected) return;
        
        _client = new Client();
        _client.Connect();
    }

    public static void Update() {
        if (_server != null) {
            _server.Update(1);
            // _server.SendToAll(new Heartbeat { Frame = _frame++, Checksum = 100}, DeliveryMethod.ReliableOrdered);
        }

        if (_client != null) {
            _client.Update(1);
        }
    }

    public static void GetGameState() {
        if(!IsClient) throw new ConstraintException("Can't get game state when not connected to server");

        Logger.Info("Getting game state");
        
        _client!.Send(new RequestGameState(), DeliveryMethod.ReliableOrdered);
    }

    public static void Disconnect() {
        _server = null;
        _client = null;
    }
}