using System.Collections.Generic;
using System.Data;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Networking; 

static class NetworkManager {

    /// <summary>
    /// Extra padding to the estimated lag to make sure we don't stutter
    /// </summary>
    private const int LagPadding = 5;
    
    private static Client? _client;
    private static Server? _server;

    public static bool IsConnected => IsClient || IsServer;
    public static bool IsClient => _client != null;
    public static bool IsServer => _server != null;
    public static bool StutterRequired => IsConnected && (IsClient ? _client!.StutterRequired: _server!.StutterRequired);
    public static int FrameDelay => IsConnected ? Latency!.Value * WarlockGame.Instance.TargetElapsedTime.Milliseconds + LagPadding : 0;
    /// <summary>
    /// Latency in milliseconds
    /// </summary>
    private static int? Latency => IsClient ? _client!.Latency : _server?.Latency;
    
    public static void StartServer() {
        if (IsConnected) return;
        
        _server = new Server();
        _server.Start();
    }

    public static void ConnectToServer(string address, string playerName) {
        if (IsConnected) return;
        
        _client = new Client();
        _client.Connect(address);
    }

    public static void Update() {
        if (_server != null) {
            _server.Update();
            _server.SendToAll(new Heartbeat { Frame = WarlockGame.Frame }, DeliveryMethod.ReliableSequenced);
        }

        if (_client != null) {
            _client.Update();
            // TODO: client can be null here if the client was disconnected during update.
            // This causes a crash if we fail to connect too.
            _client.Send(new Heartbeat { Frame = WarlockGame.Frame }, DeliveryMethod.ReliableSequenced);
        }
        
        Debug.Visualize($"Latency: {Latency}", new Vector2(800, 50));
    }

    public static void RequestGameState() {
        if(!IsClient) throw new ConstraintException("Can't get game state when not connected to server");

        Logger.Info("Getting game state");
        
        _client!.Send(new RequestGameState(), DeliveryMethod.ReliableOrdered);
    }

    public static void Disconnect() {
        _server = null;
        _client = null;
    }

    public static void SendPacket<T>(T packet) where T : INetSerializable {
        if (IsServer) {
            _server!.SendSerializableToAll(packet, DeliveryMethod.ReliableOrdered);
        }
    }

    public static void SendPlayerCommand<T>(T command) where T : INetSerializable {
        if (IsServer) {
            var packet = new PlayerInputServerResponse<T>
            {
                TargetFrame = WarlockGame.Frame + FrameDelay,
                Command = command
            };
            _server!.SendToAll(packet, DeliveryMethod.ReliableOrdered);
        }
        else {
            _client!.SendSerializable(command, DeliveryMethod.ReliableOrdered);
        }
    }
}