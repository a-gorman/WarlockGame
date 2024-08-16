using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using WarlockGame.Core.Game.Entity.Factory;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Networking;

public sealed class Client : INetEventListener {
    // LATE INIT
    private NetManager _client = null!;
    private NetPeer? _server;

    private NetDataWriter _writer = new();
    private NetPacketProcessor packetProcessor = new();

    public bool IsConnected => _server?.ConnectionState == ConnectionState.Connected;
    public int Latency { get; private set; }
    public bool StutterRequired => WarlockGame.Frame >= _maxFrameAllowed;
    private int _maxFrameAllowed;
    
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
        
        packetProcessor.RegisterCustomNestedTypes();

        packetProcessor.SubscribeReusable<Heartbeat>(OnHeartbeatReceived);
        packetProcessor.SubscribeNetSerializable<StartGame>(CommandProcessor.AddDelayedGameCommand);
        packetProcessor.SubscribeNetSerializable<PlayerCommandResponse<MoveCommand>>(OnPlayerCommandReceived);
        packetProcessor.SubscribeNetSerializable<PlayerCommandResponse<CastCommand>>(OnPlayerCommandReceived);
        packetProcessor.SubscribeNetSerializable<PlayerCommandResponse<CreateWarlock>>(OnPlayerCommandReceived);
        packetProcessor.SubscribeNetSerializable<JoinGameResponse>(OnJoinResponse);
        packetProcessor.SubscribeNetSerializable<PlayerJoined>(OnPlayerJoined);
        
        _clientConnectedCallback = clientConnectedCallback;
    }

    private void OnHeartbeatReceived(Heartbeat heartbeat) {
        _maxFrameAllowed = heartbeat.Frame;
    }

    private void OnPlayerCommandReceived<T>(PlayerCommandResponse<T> response) where T : INetSerializable, IPlayerCommand, new() {
        CommandProcessor.AddDelayedPlayerCommand(response.Command, response.TargetFrame);
    }

    private void OnPlayerJoined(PlayerJoined joinInfo) {
        PlayerManager.AddRemotePlayer(joinInfo.PlayerName);
    }
    
    private void OnJoinResponse(JoinGameResponse response) {
        Logger.Info("Joining game");
        
        foreach (var player in response.GameState.Players) {
            if (player.Id == response.PlayerId) {
                PlayerManager.AddLocalPlayer(player.Name);
            }
            else {
                PlayerManager.AddRemotePlayer(player.Name);
            }
        }

        response.GameState.Warlocks.Select(WarlockFactory.FromPacket).ForEach(EntityManager.Add);
        
        WarlockGame.Frame = response.GameState.Frame;

        // This part is a bit silly right now, but will make more sense when players don't create a warlock immediately
        // ex. waiting for games to finish, selecting stuff, delayed spawn-in, etc.
        var localPlayerId = PlayerManager.LocalPlayer!.Id;
        NetworkManager.SendPlayerCommand(new CreateWarlock { PlayerId = localPlayerId, Warlock = new Entity.Warlock(localPlayerId).Let(WarlockFactory.ToPacket)});
    }

    public void Update() {
        _client.PollEvents();
    }

    public void Send<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new() {
        if (!IsConnected) return;

        _writer.Reset();
        packetProcessor.Write(_writer, packet);
        _server!.Send(_writer, deliveryMethod);
    }

    public void SendSerializable<T>(T packet, DeliveryMethod deliveryMethod) where T : INetSerializable {
        if (!IsConnected) return;

        _writer.Reset();
        packetProcessor.WriteNetSerializable(_writer, ref packet);
        _server!.Send(_writer, deliveryMethod);
    }

    public void OnPeerConnected(NetPeer peer) {
        Logger.Info("Connected to server");
        _server = peer;

        _clientConnectedCallback?.Invoke();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Logger.Info("Peer disconnected");
        NetworkManager.Disconnect();
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
        Latency = latency;
    }

    public void OnConnectionRequest(ConnectionRequest request) {
        throw new System.NotImplementedException();
    }
}