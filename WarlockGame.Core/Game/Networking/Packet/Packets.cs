using System;
using System.Collections.Generic;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Networking.Packet;
// Disable nullability complaints. A bunch of stuff here is initialized late by deserialization
#pragma warning disable CS8618

public class JoinGameRequest : INetSerializable {
    public string PlayerName { get; set; }
    
    public Color? ColorPreference { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerName);
        writer.Put(ColorPreference?.PackedValue ?? 0);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerName = reader.GetString();
        var color = reader.GetUInt();
        ColorPreference = color == 0 ? null : new Color(color);
    }
}

// Consider rejection response. For now just let anyone in
public class JoinGameResponse : INetSerializable {
    /// <summary>
    /// The ID assigned to the joining player
    /// </summary>
    public int PlayerId { get; set; }
    public List<Player> Players { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.PutMany(Players);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        Players = reader.GetMany<Player>();
    }
}

public class PlayerJoined : INetSerializable {
    public int PlayerId { get; set; }
    public string PlayerName { get; set; }
    public Color Color { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlayerId);
        writer.Put(PlayerName);
        writer.Put(Color.PackedValue);
    }

    public void Deserialize(NetDataReader reader) {
        PlayerId = reader.GetInt();
        PlayerName = reader.GetString();
        Color = new Color(reader.GetUInt());
    }
}

public class Warlock : INetSerializable {
    public int PlayerId { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }
    public float Health { get; set; }
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
        Health = reader.GetFloat();
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
    public Color Color { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(Id);
        writer.Put(Name);
        writer.Put(Color.PackedValue);
    }

    public void Deserialize(NetDataReader reader) {
        Id = reader.GetInt();
        Name = reader.GetString();
        Color = new Color(reader.GetUInt());
    }
}

public class ServerTickProcessed : INetSerializable {
    public int Tick { get; set; }
    public int Checksum { get; set; }

    public List<IPlayerAction> PlayerCommands { get; set; } = null!;
    public List<IServerCommand> ServerCommands { get; set; } = null!;

    public void Serialize(NetDataWriter writer) {
        writer.Put(Tick);
        writer.Put(Checksum);
        PutManySeverCommands(writer, ServerCommands);
        PutManyPlayerCommands(writer, PlayerCommands);
    }

    public void Deserialize(NetDataReader reader) {
        Tick = reader.GetInt();
        Checksum = reader.GetInt();
        ServerCommands = GetManyServerCommands(reader);
        PlayerCommands = GetManyPlayerActions(reader);
    }

    private static void PutManyPlayerCommands(NetDataWriter writer, IReadOnlyCollection<IPlayerAction> commands) {
        writer.Put(commands.Count);
        foreach (var command in commands) {
            writer.Put((byte) command.GetSerializerType());
            writer.Put(command);
        }
    }

    private static void PutManySeverCommands(NetDataWriter writer, IReadOnlyCollection<IServerCommand> commands) {
        writer.Put(commands.Count);
        foreach (var command in commands) {
            writer.Put((byte) command.GetSerializerType());
            writer.Put(command);
        }
    }

    private static List<IPlayerAction> GetManyPlayerActions(NetDataReader reader) {
        var count = reader.GetInt();
        var list = new List<IPlayerAction>(count);
        for (int i = 0; i < count; i++) {
            var classType = (IPlayerAction.Type)reader.GetByte();
            switch (classType) {
                case IPlayerAction.Type.CastCommand:
                    list.Add(reader.Get<CastAction>());
                    break;
                case IPlayerAction.Type.MoveCommand:
                    list.Add(reader.Get<MoveAction>());
                    break;
                case IPlayerAction.Type.SelectPerk:
                    list.Add(reader.Get<SelectPerk>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return list;
    }

    private static List<IServerCommand> GetManyServerCommands(NetDataReader reader) {
        var count = reader.GetInt();
        var list = new List<IServerCommand>(count);
        for (int i = 0; i < count; i++) {
            var classType = (IServerCommand.Type)reader.GetByte();
            switch (classType) {
                case IServerCommand.Type.StartGame:
                    list.Add(reader.Get<StartGame>());
                    break;
                case IServerCommand.Type.PlayerRemoved:
                    list.Add(reader.Get<PlayerRemoved>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return list;
    }
}

public class ClientTickProcessed {
    public int Tick { get; set; }
    public bool ChecksumMatched { get; set; }
}


#pragma warning restore CS8618