using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using NeonShooter.Core.Game.Entity.Factory;
using NeonShooter.Core.Game.Log;

namespace NeonShooter.Core.Game.Networking; 

public sealed class Client : INetEventListener {
    // LATE INIT
    private NetManager _client = null!;
    private NetPeer? _server;
    
    private NetDataWriter _writer = new();
    private NetPacketProcessor packetProcessor = new();

    public bool IsConnected => _server?.ConnectionState == ConnectionState.Connected;
    public int Latency { get; private set; }
    public bool StutterRequired => WarlockGame.Frame >= _maxFrameAllowed;
    public bool ClientDropping => false;

    private int _maxFrameAllowed;
    public void Connect() {
        _client = new NetManager(this) {
            AutoRecycle = true,
        };

        _client.Start();
        Logger.Info("Connecting to localhost");
        _client.Connect("localhost", 12345, "");
        
        packetProcessor.RegisterWarlockNestedTypes();
        
        packetProcessor.SubscribeReusable<Heartbeat>(OnHeartbeatReceived);
        packetProcessor.SubscribeReusable<GameState>(OnGameStateReceived);
        packetProcessor.Subscribe<PlayerInputServerResponse<MoveCommand>>(OnGameCommandReceived, () => new PlayerInputServerResponse<MoveCommand>());
        packetProcessor.Subscribe<PlayerInputServerResponse<CastCommand>>(OnGameCommandReceived, () => new PlayerInputServerResponse<CastCommand>());
    }

    private void OnHeartbeatReceived(Heartbeat heartbeat) {
        _maxFrameAllowed = heartbeat.Frame;
    }

    private void OnGameCommandReceived<T>(PlayerInputServerResponse<T> serverResponse) where T:IGameCommand {
        CommandProcessor.AddDelayedGameCommand(serverResponse.Command, serverResponse.TargetFrame);
    }

    private void OnGameStateReceived(GameState gameState) {
        Logger.Info("Game state received");

        foreach (var player in gameState.Players) {
            var warlock = WarlockFactory.FromPacket(gameState.Warlocks.First(x => x.Id == player.WarlockId), player.Id);

            PlayerManager.AddRemotePlayer(new Game.Player(player.Name, player.Id, warlock));
        }

        WarlockGame.Frame = gameState.Frame;
    }

    public void Update() {
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