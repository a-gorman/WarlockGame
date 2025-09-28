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
    [InlineData(1, "name", 30)]
    [InlineData(0, "", uint.MaxValue)]
    public void PlayerSerializesCorrectly(int id, string name, uint color) {
        var player = new Player
        {
            Id = id,
            Name = name,
            Color = new Color(color)
        };

        SerializeAndDeserialize(player).Should().BeEquivalentTo(player);
    }
    
    [Fact]
    public void JoinGameRequestSerializesCorrectly() {
        var request = new JoinGameRequest
        {
            PlayerName = "name",
            ColorPreference = Color.Aquamarine
        };

        SerializeAndDeserialize(request).Should().BeEquivalentTo(request);
    }
    
    [Fact]
    public void JoinGameRequestSerializesCorrectly_NullColor() {
        var request = new JoinGameRequest
        {
            PlayerName = "name",
            ColorPreference = null
        };

        SerializeAndDeserialize(request).Should().BeEquivalentTo(request);
    }
    
    [Fact]
    public void PlayerJoinedSerializesCorrectly() {
        var request = new PlayerJoined
        {
            PlayerId = 3,
            PlayerName = "name",
            Color = Color.Aquamarine
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
        var players = new ServerTickProcessed() {
            Tick = 1,
            Checksum = 2,
            PlayerCommands = [
                new CastAction { SpellId = 3, PlayerId = 4, Type = CastAction.CastType.Directional, CastVector = new Vector2(5, 6) },
                new MoveAction { PlayerId = 7, Location = new Vector2(8, 9) }
            ],
            ServerCommands = [
                new StartGame()
            ]
        };

        SerializeAndDeserialize(players).Should().BeEquivalentTo(players);
    }

    private T SerializeAndDeserialize<T>(T packet) where T : class, INetSerializable, new() {
        var writer = new NetDataWriter();
        packet.Serialize(writer);
        return new NetDataReader(writer.CopyData()).Get(() => new T());
    }
}