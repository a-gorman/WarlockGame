using System.Collections.Generic;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Networking;
// Disable nullability complaints. A bunch of stuff here is initialized late by deserialization
#pragma warning disable CS8618

public class GameState : INetSerializable {
    public List<Player> Players { get; set; }
    public List<Warlock> Warlocks { get; set; }
    public int Frame { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.PutMany(Players);
        writer.PutMany(Warlocks);
        writer.Put(Frame);
    }

    public void Deserialize(NetDataReader reader) {
        Players = reader.GetMany<Player>();
        Warlocks = reader.GetMany<Warlock>();
        Frame = reader.GetInt();
    }
}

public class JoinGameRequest : INetSerializable {
    public string PlayerName { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerName);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerName = reader.GetString();
    }
}

// Consider rejection response. For now just let anyone in
public class JoinGameResponse : INetSerializable {
    /// <summary>
    /// The ID assigned to the joining player
    /// </summary>
    public int PlayerId { get; set; }
    
    public GameState GameState { get; set; }

public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(GameState);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        GameState = reader.Get<GameState>();
    }
}

public class PlayerJoined : INetSerializable {
    public string PlayerName { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerName);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerName = reader.GetString();
    }
}

public class CreateWarlock : IPlayerCommand, INetSerializable {
    public int PlayerId { get; set; }
    public Warlock Warlock { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(Warlock);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        Warlock = reader.Get<Warlock>();
    }
}

public class Warlock : INetSerializable {
    public int PlayerId { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }
    public int Health { get; set; }
    public List<Spell> Spells { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(Position);
        writer.Put(Velocity);
        writer.Put(Orientation);
        writer.Put(Health);
        writer.PutMany(Spells);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        Position = reader.GetVector2();
        Velocity = reader.GetVector2();
        Orientation = reader.GetFloat();
        Health = reader.GetInt();
        Spells = reader.GetMany<Spell>();
    }
}

public class Spell : INetSerializable {
    public int SpellId { get; set; }
    public int CooldownRemaining { get; set; }
    public void Serialize(NetDataWriter writer) {
        writer.Put(SpellId);
        writer.Put(CooldownRemaining);
    }

    public void Deserialize(NetDataReader reader) {
        SpellId = reader.GetInt();
        CooldownRemaining = reader.GetInt();
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

public class MoveCommand : IPlayerCommand, INetSerializable {
    
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

public class CastCommand : IPlayerCommand, INetSerializable {
    
    public int PlayerId { get; set; }
    
    public Vector2 CastVector { get; set; }
    
    public int SpellId { get; set; }
    
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

public class PlayerCommandResponse<T> : INetSerializable, ISynchronizedCommand where T : INetSerializable, IPlayerCommand, new() {
    public int TargetFrame { get; set; }
    
    public T Command { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(TargetFrame);
        writer.Put(Command);
    }

    public void Deserialize(NetDataReader reader) {
        TargetFrame = reader.GetInt();
        Command = reader.Get<T>();
    }
}

public class StartGame : ISynchronizedCommand, INetSerializable {
    public int TargetFrame { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(TargetFrame);
    }

    public void Deserialize(NetDataReader reader) {
        TargetFrame = reader.GetInt();
    }
}

public class RequestGameState {
    public int Frame { get; set; }
}

public class Heartbeat {
    public int Frame { get; set; }
}

/// <summary>
/// A player issued command that has an effect on the game.
/// For example, issuing orders to a warlock or pausing the game.
/// </summary>
public interface IPlayerCommand {
    int PlayerId { get; }
}

/// <summary>
/// Server commands that need to be synchronized to a game frame
/// </summary>
public interface ISynchronizedCommand {
    int TargetFrame { get; }
}

#pragma warning restore CS8618