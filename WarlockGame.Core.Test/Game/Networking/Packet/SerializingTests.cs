using System.Collections.Generic;
using FluentAssertions;
using JetBrains.Annotations;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Networking.Packet;
using Xunit;

namespace WarlockGame.Core.Test.Game.Networking.Packet;

[TestSubject(typeof(Player))]
public class SerializingTests {

    [Theory]
    [InlineData(1, "name")]
    [InlineData(0, "")]
    public void PlayerSerializesCorrectly(int id, string name) {
        var player = new Player
        {
            Id = id,
            Name = name
        };

        SerializeAndDeserialize(player).Should().BeEquivalentTo(player);
    }
    
    [Fact]
    public void JoinGameRequestSerializesCorrectly() {
        var request = new JoinGameRequest
        {
            PlayerName = "name"
        };

        SerializeAndDeserialize(request).Should().BeEquivalentTo(request);
    }

    [Fact]
    public void JoinGameResponseSerializesCorrectly() {
        var players = new JoinGameResponse
        {
            PlayerId = 1,
            Players = new List<Player>
            {
                new() {Id = 2, Name = "1"},
                new() {Id = 3, Name = "2"}
            }
        };

        SerializeAndDeserialize(players).Should().BeEquivalentTo(players);
    }
    
    [Fact]
    public void ServerTickProcessedSerializesCorrectly() {
        var players = new ServerTickProcessed()
        {
            Tick = 1,
            Checksum = 2,
            PlayerCommands = new List<IPlayerCommand>
            {
                new CastCommand { SpellId = 3, PlayerId = 4, CastVector = new Vector2(5, 6) },
                new MoveCommand { PlayerId = 7, Location = new Vector2(8, 9) }
            }
        };

        SerializeAndDeserialize(players).Should().BeEquivalentTo(players);
    }

    private T SerializeAndDeserialize<T>(T packet) where T : class, INetSerializable, new() {
        var writer = new NetDataWriter();
        packet.Serialize(writer);
        return new NetDataReader(writer.CopyData()).Get(() => new T());
    }
}