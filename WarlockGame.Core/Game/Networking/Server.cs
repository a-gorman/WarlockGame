using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using WarlockGame.Core.Game.Entity.Factory;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Networking;

public class Server : INetEventListener {
    // LATE INIT
    private NetManager _server = null!;
    private readonly NetDataWriter _writer = new();
    private readonly NetPacketProcessor _packetProcessor = new();

    private readonly Dictionary<int, ClientPeer> _clients = new();

    public bool StutterRequired { get; private set; }
    public bool ClientDropping { get; private set; }
    public int Latency { get; private set; }

    public void Start() {
        _server = new NetManager(this)
        {
            AutoRecycle = true
        };

        Logger.Info("Starting server");
        _server.Start(6112);
        Logger.Info($"Server stated. IPAddress: { NetUtils.GetLocalIpList(LocalAddrType.IPv4).JoinToString() }");

        _packetProcessor.RegisterCustomNestedTypes();

        _packetProcessor.SubscribeReusable<RequestGameState, NetPeer>(OnRequestGameState);
        _packetProcessor.SubscribeNetSerializable<MoveCommand>(OnGameCommandReceived);
        _packetProcessor.SubscribeNetSerializable<CastCommand>(OnGameCommandReceived);
        _packetProcessor.SubscribeNetSerializable<CreateWarlock>(OnGameCommandReceived);
        _packetProcessor.SubscribeReusable<Heartbeat, NetPeer>(OnHeartbeatReceived);
        _packetProcessor.SubscribeReusable<JoinGameRequest, NetPeer>(OnJoinGameRequest);
    }

    private void OnHeartbeatReceived(Heartbeat heartbeat, NetPeer sender) {
        if (_clients.TryGetValue(sender.Id, out var peer)) {
            peer.Frame = heartbeat.Frame;
        }
    }

    private void OnJoinGameRequest(JoinGameRequest request, NetPeer sender) {
        var player = PlayerManager.AddRemotePlayer(request.PlayerName);
        
        SendToAllExcept(new PlayerJoined { PlayerName = request.PlayerName }, sender, DeliveryMethod.ReliableOrdered);
        SendToPeer(new JoinGameResponse { PlayerId = player.Id, GameState = CreateGameState() }, sender, DeliveryMethod.ReliableOrdered);
    }
    
    private void OnGameCommandReceived<T>(T request) where T : IPlayerCommand, INetSerializable, new() {
        var targetFrame = WarlockGame.Frame + NetworkManager.FrameDelay;

        var action = new PlayerCommandResponse<T>
        {
            Command = request,
            TargetFrame = targetFrame
        };
        CommandProcessor.AddDelayedPlayerCommand(request, targetFrame);
        SendSerializableToAll(action, DeliveryMethod.ReliableOrdered);
    }

    private void OnRequestGameState(RequestGameState _, NetPeer peer) {
        Logger.Info("Game state request received");
        SendToPeer(CreateGameState(), peer, DeliveryMethod.ReliableOrdered);
    }

    private static GameState CreateGameState() {
        var gameState = new GameState
        {
            Players = PlayerManager.Players
                                   .Select(x => new Player { Name = x.Name, Id = x.Id })
                                   .ToList(),
            Warlocks = EntityManager.Warlocks.Select(WarlockFactory.ToPacket).ToList(),
            Frame = WarlockGame.Frame
        };
        return gameState;
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
            } else if(WarlockGame.Frame - client.Frame > 10) {
                Logger.Info("Lag detected");
                StutterRequired = true;
            }
            else if(client.NetPeer.TimeSinceLastPacket > 1000) { // 1s
                Logger.Info("Client is dropping");
                ClientDropping = true;
            }
        }
    }

    public void SendToAll<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new() {
        _writer.Reset();
        _packetProcessor.Write(_writer, packet);
        _server.SendToAll(_writer, deliveryMethod);
    }
    
    public void SendToAllExcept<T>(T packet, NetPeer excluded, DeliveryMethod deliveryMethod) where T : class, new() {
        _writer.Reset();
        _packetProcessor.Write(_writer, packet);
        _server.SendToAll(_writer, deliveryMethod, excluded);
    }
    
    public void SendToPeer<T>(T packet, NetPeer peer, DeliveryMethod deliveryMethod) where T : class, new() {
        _writer.Reset();
        _packetProcessor.Write(_writer, packet);
        peer.Send(_writer, deliveryMethod);
    }

    public void SendSerializableToAll<T>(T packet, DeliveryMethod deliveryMethod) where T : INetSerializable {
        _writer.Reset();
        _packetProcessor.WriteNetSerializable(_writer, ref packet);
        _server.SendToAll(_writer, deliveryMethod);
    }

    public void OnPeerConnected(NetPeer peer) {
        Logger.Info($"Peer connected: {peer.Id}");
        _clients.Add(peer.Id, new ClientPeer { NetPeer = peer });
        CalculateLatency();
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
        _packetProcessor.ReadAllPackets(reader, peer);
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
        return _clients.Values.Select(x => x.Latency).DefaultIfEmpty().Max();
    }

    private class ClientPeer {
        public required NetPeer NetPeer { get; init; }
        public int Latency { get; set; }
        public int Frame { get; set; }
    }
}