using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking.Packet;

namespace WarlockGame.Core.Game.Networking;

sealed class Client : INetEventListener {
    // LATE INIT
    private NetManager _client = null!;
    private NetPeer? _server;

    private readonly NetDataWriter _writer = new();
    private readonly NetPacketProcessor _packetProcessor = new();

    public bool IsConnected => _server?.ConnectionState == ConnectionState.Connected;
    public int Latency { get; private set; }
    
    private Action? _clientConnectedCallback = null;

    public void Connect(string address, Action clientConnectedCallback) {
        if (_clientConnectedCallback != null) {
            Logger.Warning("Tried connecting to server while already trying to connect");
            return;
        }
        
        _client = new NetManager(this)
        {
            AutoRecycle = true,
        };

        _client.Start();
        Logger.Info($"Connecting to {address}");
        _client.Connect(address, 6112, "");
        
        _packetProcessor.RegisterCustomNestedTypes();

        _packetProcessor.SubscribeNetSerializable(OnServerTickProcessed, () => new ServerTickProcessed());
        _packetProcessor.SubscribeNetSerializable(OnJoinResponse, () => new JoinGameResponse());
        _packetProcessor.SubscribeNetSerializable(OnPlayerJoined, () => new PlayerJoined());
        _packetProcessor.SubscribeNetSerializable(OnStartGame,() => new StartGame());
        
        _clientConnectedCallback = clientConnectedCallback;
    }

    private void OnStartGame(StartGame packet) {
        WarlockGame.Instance.RestartGame(packet.Seed);
    }

    private void OnServerTickProcessed(ServerTickProcessed serverTickProcessed) {
        WarlockGame.Instance.OnServerTickProcessed(serverTickProcessed);
    }

    private void OnPlayerJoined(PlayerJoined joinInfo) {
        PlayerManager.AddRemotePlayer(joinInfo.PlayerName);
    }
    
    private void OnJoinResponse(JoinGameResponse response) {
        Logger.Info("Joining game");
        
        foreach (var player in response.Players) {
            if (player.Id == response.PlayerId) {
                PlayerManager.AddLocalPlayer(player.Name);
            }
            else {
                PlayerManager.AddRemotePlayer(player.Name);
            }
        }
    }

    public void Update() {
        _client.PollEvents();
    }

    public void Send<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new() {
        if (!IsConnected) return;

        _writer.Reset();
        _packetProcessor.Write(_writer, packet);
        _server!.Send(_writer, deliveryMethod);
    }

    public void SendSerializable<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable {
        if (!IsConnected) return;

        _writer.Reset();
        _packetProcessor.WriteNetSerializable(_writer, ref packet);
        _server!.Send(_writer, deliveryMethod);
    }

    public void OnPeerConnected(NetPeer peer) {
        Logger.Info("Connected to server");
        _server = peer;

        _clientConnectedCallback?.Invoke();
        _clientConnectedCallback = null;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Logger.Info("Peer disconnected");
        NetworkManager.Disconnect();
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) {
        throw new NotImplementedException();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
        DeliveryMethod deliveryMethod) {
        _packetProcessor.ReadAllPackets(reader, peer);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType) {
        throw new NotImplementedException();
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
        Latency = latency;
    }

    public void OnConnectionRequest(ConnectionRequest request) {
        throw new NotImplementedException();
    }
}