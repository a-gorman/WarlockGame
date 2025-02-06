using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Networking.Packet;

/// <summary>
/// A player issued command that has an effect on the game.
/// For example, issuing orders to a warlock or pausing the game.
/// </summary>
public interface IPlayerCommand : INetSerializable {
    int PlayerId { get; }

    enum Type {
        MoveCommand,
        CastCommand,
    }

    Type GetSerializerType();
}

public class MoveCommand : IPlayerCommand {
    public int PlayerId { get; set; }

    public Vector2 Location { get; set; }

    public IPlayerCommand.Type GetSerializerType() => IPlayerCommand.Type.MoveCommand;

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(Location);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        Location = reader.GetVector2();
    }
}

public class CastCommand : IPlayerCommand {
    public int PlayerId { get; set; }
    public Vector2 CastVector { get; set; }

    public int SpellId { get; set; }

    public IPlayerCommand.Type GetSerializerType() => IPlayerCommand.Type.CastCommand;

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(CastVector);
        writer.Put(SpellId);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        CastVector = reader.GetVector2();
        SpellId = reader.GetInt();
    }
}