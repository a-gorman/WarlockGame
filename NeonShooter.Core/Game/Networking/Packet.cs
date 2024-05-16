using System.Linq;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Networking;
// Disable nullability complaints. A bunch of stuff here is initialized late by deserialization
#pragma warning disable CS8618

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
    public int Health { get; set; }
    public int[] SpellIds { get; set; }
    /// <summary>
    /// Cooldown remaining in frames
    /// Has same length as SpellIds
    /// </summary>
    public int[] SpellCooldowns { get; set; }
    public void Serialize(NetDataWriter writer) {
        writer.Put(Id);
        writer.Put(Position);
        writer.Put(Velocity);
        writer.Put(Orientation);
        writer.Put(Health);
        writer.PutArray(SpellIds);
        writer.PutArray(SpellCooldowns);
    }

    public void Deserialize(NetDataReader reader) {
        Id = reader.GetInt();
        Position = reader.GetVector2();
        Velocity = reader.GetVector2();
        Orientation = reader.GetFloat();
        Health = reader.GetInt();
        SpellIds = reader.GetIntArray();
        SpellCooldowns = reader.GetIntArray();
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

public class MoveCommand : IGameCommand, INetSerializable {
    
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

public class CastCommand : IGameCommand, INetSerializable {
    
    public int PlayerId { get; set; }
    
    public Vector2 Location { get; set; }
    
    public int SpellId { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(Location);
        writer.Put(SpellId);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        Location = reader.GetVector2();
        SpellId = reader.GetInt();
    }
}

public class PlayerInputServerResponse<T> where T : notnull {
    public int TargetFrame { get; set; }
    
    public T Command { get; set; }
} 

public class RequestGameState { }

// public class ClientHeartbeat {
//     public int PlayerId { get; set; }
//     public int Frame { get; set; }
// }

public class Heartbeat {
    public int Frame { get; set; }
}

/// <summary>
/// A player issued command that has an effect on the game.
/// For example, issuing orders to a warlock or pausing the game.
/// </summary>
public interface IGameCommand {
    int PlayerId { get; set; }
}
#pragma warning restore CS8618
