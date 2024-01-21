using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Networking;

public class GameState {
    public Player[] Players { get; init; }
    public Warlock[] Warlocks { get; init; }
}

public class Warlock : INetSerializable {
    public int Id { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }
    public void Serialize(NetDataWriter writer) {
        writer.Put(Id);
        writer.Put(Position);
        writer.Put(Velocity);
        writer.Put(Orientation);
    }

    public void Deserialize(NetDataReader reader) {
        Id = reader.GetInt();
        Position = reader.GetVector2();
        Velocity = reader.GetVector2();
        Orientation = reader.GetFloat();
    }
}

public class Player : INetSerializable {
    public string Name { get; set; }
    public int Id { get; set; }
    public int WarlockId { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(Id);
        writer.Put(Name);
        writer.Put(WarlockId);
    }

    public void Deserialize(NetDataReader reader) {
        Id = reader.GetInt();
        Name = reader.GetString();
        WarlockId = reader.GetInt();
    }
}

public class RequestGameState { }

public class Heartbeat {
    public uint Frame { get; set; }
    public uint Checksum { get; set; }
}