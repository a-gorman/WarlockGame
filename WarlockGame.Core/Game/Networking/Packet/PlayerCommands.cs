using LiteNetLib.Utils;

namespace WarlockGame.Core.Game.Networking.Packet;

/// <summary>
/// A player issued command that has an effect on the game.
/// For example, issuing orders to a warlock or pausing the game.
/// </summary>
public interface IPlayerAction : INetSerializable {
    int PlayerId { get; }

    enum Type {
        MoveCommand,
        CastCommand,
        SelectPerk
    }

    Type GetSerializerType();
}

public class MoveAction : IPlayerAction {
    public int PlayerId { get; set; }

    public Vector2 Location { get; set; }

    public IPlayerAction.Type GetSerializerType() => IPlayerAction.Type.MoveCommand;

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(Location);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        Location = reader.GetVector2();
    }
}

public class CastAction : IPlayerAction {
    public int PlayerId { get; set; }
    public Vector2 CastVector { get; set; }

    public int SpellId { get; set; }
    public CastType Type { get; set; }

    public enum CastType {
        Invalid = 0,
        Self = 1,
        Location = 2,
        Directional = 3
    }

    public IPlayerAction.Type GetSerializerType() => IPlayerAction.Type.CastCommand;

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(CastVector);
        writer.Put(SpellId);
        writer.Put((int)Type);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        CastVector = reader.GetVector2();
        SpellId = reader.GetInt();
        Type = (CastType) reader.GetInt();
    }
}

class SelectPerk : IPlayerAction {
    public int PlayerId { get; set; }
    public int PerkId { get; set; }

    public IPlayerAction.Type GetSerializerType() => IPlayerAction.Type.SelectPerk;

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(PerkId);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        PerkId = reader.GetInt();
    }
}