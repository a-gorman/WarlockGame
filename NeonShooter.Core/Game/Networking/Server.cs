using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Entity.Factory;
using NeonShooter.Core.Game.Log;

namespace NeonShooter.Core.Game.Networking;

public class Server : INetEventListener {
    // LATE INIT
    private NetManager _server = null!;
    private NetDataWriter _writer = new();
    private NetPacketProcessor packetProcessor = new();
    
    private readonly Dictionary<int, NetPeer> _clients = new();

    public int Latency { get; private set; }

    public void Start() {
        _server = new NetManager(this) {
            AutoRecycle = true,
        };
        
        Logger.Info("Starting server");
        var connected = _server.Start(12345);
        Logger.Info($"Server stated: {connected}");
        
        packetProcessor.RegisterNestedType<Vector2>((w, v) => w.Put(v), reader => reader.GetVector2());
        packetProcessor.RegisterNestedType<Warlock>(() => new Warlock());
        packetProcessor.RegisterNestedType<Player>(() => new Player());
        
        packetProcessor.Subscribe<RequestGameState>(OnRequestGameState, () => new RequestGameState());
    }

    public void OnRequestGameState(RequestGameState _) {
        Logger.Info("Game state request received");
        
        var gameState = new GameState
        {
            Players = PlayerManager.Players.Select(x => new Networking.Player { Name = x.Name, Id = x.Id, WarlockId = x.Warlock.Id }).ToArray(),
            Warlocks = PlayerManager.Players.Select(x => WarlockFactory.ToPacket(x.Warlock)).ToArray()
        };
        
        SendToAll(gameState, DeliveryMethod.ReliableOrdered);
    }
    
    public void OnConnectionRequest(ConnectionRequest request) {
        Logger.Info($"Incoming connection from {request.RemoteEndPoint}");
        request.Accept();
    }

    public void Update(float delta) {
        _server.PollEvents();
    }

    public void SendToAll<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new() {
        _writer.Reset();
        packetProcessor.Write(_writer, packet);
        _server.SendToAll(_writer, deliveryMethod);
    }
    
    public void OnPeerConnected(NetPeer peer) {
        Logger.Info($"Peer connected: {peer.Id}");
        _clients.Add(peer.Id, peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Logger.Info($"Peer disconnected: {peer.Id}");
        _clients.Remove(peer.Id);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) {
        throw new System.NotImplementedException();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) {
        packetProcessor.ReadAllPackets(reader, peer);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) {
        throw new System.NotImplementedException();
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
        Latency = latency;
    }
}