using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Entity.Factory;
using NeonShooter.Core.Game.Log;
using NeonShooter.Core.Game.Util;
using NeonShooter.Core.Game.UX.InputDevices;

namespace NeonShooter.Core.Game.Networking; 

public sealed class Client : INetEventListener {
    // LATE INIT
    private NetManager _client = null!;
    private NetPeer? _server;
    
    private NetDataWriter _writer = new();
    private NetPacketProcessor packetProcessor = new();

    public bool IsConnected => _server?.ConnectionState == ConnectionState.Connected;
    public int Latency { get; private set; }

    public void Connect() {
        _client = new NetManager(this) {
            AutoRecycle = true,
        };

        _client.Start();
        Logger.Info("Connecting to localhost");
        _client.Connect("localhost", 12345, "");
        
        packetProcessor.RegisterWarlockNestedTypes();
        
        packetProcessor.SubscribeReusable<Heartbeat>(x => Logger.Info($"Heartbeat received: Frame: {x.Frame} Checksum: {x.Checksum}"));
        packetProcessor.SubscribeReusable<GameState>(OnGameStateReceived);
        packetProcessor.Subscribe<PlayerInputResponse<MoveAction>>(OnMoveCommandReceived, () => new PlayerInputResponse<MoveAction>());
    }

    private void OnMoveCommandReceived(PlayerInputResponse<MoveAction> response) {
        InputManager.AddMoveAction(response.Command, response.TargetFrame);
    }

    private void OnGameStateReceived(GameState gameState) {
        Logger.Info("Game state received");
        
        var players = gameState.Players.Select(x => new Game.Player(x.Name, x.Id, Array.Empty<IInputDevice>())).ToDictionary(x => x.Id);
        
        var warlocks = gameState.Warlocks.Select(x => WarlockFactory.FromPacket(x, players[gameState.Players.First(y => y.WarlockId == x.Id).Id])).ToArray();
        players.Values.OnEach(x => x.Warlock = warlocks.First(y => y.PlayerId == x.Id)).ForEach(PlayerManager.AddPlayer);
        WarlockGame.Frame = gameState.Frame;
    }

    public void Update(float delta) {
        _client.PollEvents();
    }

    public void Send<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new() {
        if(!IsConnected) return;
        
        _writer.Reset();
        packetProcessor.Write(_writer, packet);
        _server!.Send(_writer, deliveryMethod);
    }

    public void SendSerializable<T>(T packet, DeliveryMethod deliveryMethod) where T : INetSerializable {
        if(!IsConnected) return;
        
        _writer.Reset();
        packetProcessor.WriteNetSerializable(_writer, ref packet);
        _server!.Send(_writer, deliveryMethod);
    }
    
    public void OnPeerConnected(NetPeer peer) {
        Logger.Info("Connected to server");
        _server = peer;
        
        NetworkManager.RequestGameState();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Logger.Info("Peer disconnected");
        NetworkManager.Disconnect();
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

    public void OnConnectionRequest(ConnectionRequest request) {
        throw new System.NotImplementedException();
    }
}