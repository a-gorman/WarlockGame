using LiteNetLib;

namespace NeonShooter.Core.Game.Networking; 

static class NetworkManager {

    private static Client? _client;
    private static Server? _server;

    private static uint _frame = 0;
    
    public static void StartServer() {
        _server = new Server();
        _server.Start();
    }

    public static void ConnectToServer() {
        _client = new Client();
        _client.Connect();
    }

    public static void Update() {
        if (_server != null) {
            _server.Update(1);
            _server.SendToAll(new Heartbeat { Frame = _frame++, Checksum = 100}, DeliveryMethod.ReliableOrdered);
        }

        if (_client != null) {
            _client.Update(1);
        }
    }
}