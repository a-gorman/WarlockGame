using LiteNetLib.Utils;

namespace WarlockGame.Core.Game.Networking.Packet;

public interface IServerCommand : INetSerializable {
    enum Type {
        StartGame,
        PlayerRemoved
    }

    Type GetSerializerType();
}

class StartGame : IServerCommand {
    public void Serialize(NetDataWriter writer) { }

    public void Deserialize(NetDataReader reader) { }
    public IServerCommand.Type GetSerializerType() => IServerCommand.Type.StartGame;
}

public class PlayerRemoved : IServerCommand {
    public int PlayerId { get; set; }

    public IServerCommand.Type GetSerializerType() => IServerCommand.Type.StartGame;
    public void Serialize(NetDataWriter writer) { }

    public void Deserialize(NetDataReader reader) { }
}