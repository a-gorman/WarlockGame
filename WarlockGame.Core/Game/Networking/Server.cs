using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Networking;

class Server : INetEventListener {
    // LATE INIT
    private NetManager _server = null!;
    private readonly NetDataWriter _writer = new();
    private readonly NetPacketProcessor _packetProcessor = new();

    private readonly Dictionary<int, ClientPeer> _clients = new();
    private readonly Dictionary<int, int> _clientToPlayerMap = new();

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

        _packetProcessor.SubscribeNetSerializable(OnGameCommandReceived, () => new MoveCommand());
        _packetProcessor.SubscribeNetSerializable(OnGameCommandReceived, () => new CastCommand());
        _packetProcessor.SubscribeReusable<ClientTickProcessed, NetPeer>(OnClientTickProcessed);
        _packetProcessor.SubscribeReusable<JoinGameRequest, NetPeer>(OnJoinGameRequest);
    }

    private void OnClientTickProcessed(ClientTickProcessed clientTickProcessed, NetPeer sender) {
        WarlockGame.Instance.ClientTickProcessed(_clientToPlayerMap[sender.Id], clientTickProcessed);
    }

    private void OnJoinGameRequest(JoinGameRequest request, NetPeer sender) {
        var player = PlayerManager.AddRemotePlayer(request.PlayerName);
        _clientToPlayerMap.Add(sender.Id, player.Id);
        SendToAllExcept(new PlayerJoined { PlayerName = request.PlayerName }, sender);
        SendToPeer(new JoinGameResponse { PlayerId = player.Id, Players = PlayerManager.Players.Select(x => new Packet.Player { Id = x.Id, Name = x.Name }).ToList() }, sender);

        if (WarlockGame.Instance.Config.RestartOnJoin) {
            CommandManager.AddServerCommand(new StartGame { Seed = Random.Shared.Next() });
        }
    }
    
    private void OnGameCommandReceived<T>(T request) where T : IPlayerCommand, INetSerializable, new() {
        CommandManager.AddSimulationCommand(request);
    }

    public void OnConnectionRequest(ConnectionRequest request) {
        Logger.Info($"Incoming connection from {request.RemoteEndPoint}");
        request.Accept();
    }

    public void Update() {
        _server.PollEvents();

        ClientDropping = false;
        foreach (var client in _clients.Values) {
            if(client.NetPeer.TimeSinceLastPacket > 1000) { // 1s
                // MessageDisplay.Display("Client is dropping");
                ClientDropping = true;
            }
        }
    }

    public void SendToAll<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new() {
        _writer.Reset();
        _packetProcessor.Write(_writer, packet);
        _server.SendToAll(_writer, deliveryMethod);
    }
    
    public void SendToAllExcept<T>(T packet, NetPeer excluded, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new() {
        _writer.Reset();
        _packetProcessor.Write(_writer, packet);
        _server.SendToAll(_writer, deliveryMethod, excluded);
    }
    
    public void SendToPeer<T>(T packet, NetPeer peer, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable {
        _writer.Reset();
        _packetProcessor.WriteNetSerializable(_writer, ref packet);
        peer.Send(_writer, deliveryMethod);
    }

    public void SendSerializableToAll<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable {
        _writer.Reset();
        _packetProcessor.WriteNetSerializable(_writer, ref packet);
        _server.SendToAll(_writer, deliveryMethod);
    }

    public void OnPeerConnected(NetPeer peer) {
        Logger.Info($"Peer connected: {peer.Id}");
        MessageDisplay.Display($"Peer connected: {peer.Id}");
        _clients.Add(peer.Id, new ClientPeer { NetPeer = peer });
        CalculateLatency();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Logger.Info($"Peer disconnected: {peer.Id}");
        MessageDisplay.Display($"Peer disconnected: {peer.Id}");
        _clients.Remove(peer.Id);
        Latency = CalculateLatency();

        var packet = new PlayerRemoved { PlayerId = _clientToPlayerMap[peer.Id] };
        SendToAll(packet);
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
    }
}