using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Networking;

public class GameState {
    public Player[] Players { get; init; }
    public Warlock[] Warlocks { get; init; }
    public int Frame { get; init; }
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
    public int Id { get; set; }
    public string Name { get; set; }
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

public class MoveAction : INetSerializable {
    
    public int PlayerId { get; set; }
    
    public Vector2 Location { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(Location);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        Location = reader.GetVector2();
    }
}

public class PlayerInputResponse<T> where T : notnull {
    public int TargetFrame { get; set; }
    
    public T Command { get; set; }
} 

public class RequestGameState { }

public class Heartbeat {
    public uint Frame { get; set; }
    public uint Checksum { get; set; }
}