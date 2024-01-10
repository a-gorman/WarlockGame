using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using NeonShooter.Core.Game.Log;

namespace NeonShooter.Core.Game.Networking; 

public class Client : INetEventListener {
    // LATE INIT
    private NetManager _client = null!;
    private NetPeer? _server;
    
    private NetDataWriter _writer = new();
    private NetPacketProcessor packetProcessor = new();

    public void Connect() {
        _client = new NetManager(this) {
            AutoRecycle = true,
        };

        _client.Start();
        Logger.Info("Connecting to localhost");
        _client.Connect("localhost", 12345, "");
        packetProcessor.SubscribeReusable<Heartbeat>(x => Logger.Info($"Heartbeat received: Frame: {x.Frame} Checksum: {x.Checksum}"));
    }

    public void Update(float delta) {
        _client.PollEvents();
    }

    public void OnPeerConnected(NetPeer peer) {
        Logger.Info("Connected to server");
        _server = peer;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        throw new System.NotImplementedException();
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
        // throw new System.NotImplementedException();
    }

    public void OnConnectionRequest(ConnectionRequest request) {
        throw new System.NotImplementedException();
    }
}