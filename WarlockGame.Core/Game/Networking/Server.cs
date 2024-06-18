using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity.Factory;
using WarlockGame.Core.Game.Log;

namespace WarlockGame.Core.Game.Networking;

public class Server : INetEventListener {
    // LATE INIT
    private NetManager _server = null!;
    private NetDataWriter _writer = new();
    private NetPacketProcessor packetProcessor = new();

    private readonly Dictionary<int, ClientPeer> _clients = new();

    public bool StutterRequired { get; private set; }
    public bool ClientDropping { get; private set; }
    public int Latency { get; private set; }

    public void Start() {
        _server = new NetManager(this)
        {
            AutoRecycle = true,
        };

        Logger.Info("Starting server");
        var connected = _server.Start(12345);
        Logger.Info($"Server stated: {connected}");

        packetProcessor.RegisterWarlockNestedTypes();

        packetProcessor.Subscribe<RequestGameState>(OnRequestGameState, () => new RequestGameState());
        packetProcessor.Subscribe<MoveCommand>(OnGameCommandReceived, () => new MoveCommand());
        packetProcessor.Subscribe<CastCommand>(OnGameCommandReceived, () => new CastCommand());
        // packetProcessor.Subscribe<Heartbeat>(OnHeartbeatReceived, () => new Heartbeat());
        packetProcessor.SubscribeReusable<Heartbeat, NetPeer>(OnHeartbeatReceived);
    }

    private void OnHeartbeatReceived(Heartbeat heartbeat, NetPeer sender) {
        if (_clients.TryGetValue(sender.Id, out var peer)) {
            peer.Frame = heartbeat.Frame;
        }
    }

    private void OnGameCommandReceived<T>(T request) where T : IGameCommand {
        var targetFrame = WarlockGame.Frame + NetworkManager.FrameDelay;

        var moveAction = new PlayerInputServerResponse<T>
        {
            Command = request,
            TargetFrame = targetFrame
        };
        CommandProcessor.AddDelayedGameCommand(request, targetFrame);
        SendToAll(moveAction, DeliveryMethod.ReliableOrdered);
    }

    private void OnRequestGameState(RequestGameState _) {
        Logger.Info("Game state request received");

        var gameState = new GameState
        {
            Players = PlayerManager.Players
                                   .Select(x => new Player { Name = x.Name, Id = x.Id, WarlockId = x.Warlock.Id })
                                   .ToArray(),
            Warlocks = PlayerManager.Players.Select(x => WarlockFactory.ToPacket(x.Warlock)).ToArray(),
            Frame = WarlockGame.Frame
        };

        SendToAll(gameState, DeliveryMethod.ReliableOrdered);
    }

    public void OnConnectionRequest(ConnectionRequest request) {
        Logger.Info($"Incoming connection from {request.RemoteEndPoint}");
        request.Accept();
    }

    public void Update() {
        _server.PollEvents();

        ClientDropping = false;
        StutterRequired = false;
        foreach (var client in _clients.Values) {
            if (client.Frame > WarlockGame.Frame) {
                Logger.Warning($"Client {client.NetPeer.Id} ahead of server");
            } else if(WarlockGame.Frame - client.Frame > 100) {
                Logger.Info($"Lag detected");
                StutterRequired = true;
            }
            else if(client.NetPeer.TimeSinceLastPacket > 1000) { // 1s
                Logger.Info($"Client is dropping");
                ClientDropping = true;
            }
        }

    }

    public void SendToAll<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new() {
        _writer.Reset();
        packetProcessor.Write(_writer, packet);
        _server.SendToAll(_writer, deliveryMethod);
    }

    public void SendSerializableToAll<T>(T packet, DeliveryMethod deliveryMethod) where T : INetSerializable {
        _writer.Reset();
        packetProcessor.WriteNetSerializable(_writer, ref packet);
        _server.SendToAll(_writer, deliveryMethod);
    }

    public void OnPeerConnected(NetPeer peer) {
        Logger.Info($"Peer connected: {peer.Id}");
        _clients.Add(peer.Id, new ClientPeer { NetPeer = peer });
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Logger.Info($"Peer disconnected: {peer.Id}");
        _clients.Remove(peer.Id);
        Latency = CalculateLatency();
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) {
        throw new System.NotImplementedException();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
        DeliveryMethod deliveryMethod) {
        packetProcessor.ReadAllPackets(reader, peer);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType) {
        throw new System.NotImplementedException();
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
        _clients[peer.Id].Latency = latency;
        Latency = CalculateLatency();
    }

    private int CalculateLatency() {
        return _clients.Count > 0 ? _clients.Values.Max(x => x.Latency) : 0;
    }

    private class ClientPeer {
        public required NetPeer NetPeer { get; init; }
        public int Latency { get; set; }
        public int Frame { get; set; }
    }
}