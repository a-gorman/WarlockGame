using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Networking.Packet;

namespace WarlockGame.Core.Game.Networking; 

public static class NetworkExtensions {
    public static void RegisterCustomNestedTypes(this NetPacketProcessor processor) {
        processor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
        processor.RegisterNestedType(() => new Warlock());
        processor.RegisterNestedType(() => new Packet.Player());
        processor.RegisterNestedType(() => new MoveCommand());
        processor.RegisterNestedType(() => new CastCommand());
        processor.RegisterNestedType(() => new CreateWarlock());
        processor.RegisterNestedType(() => new StartGame());
        processor.RegisterNestedType(() => new PlayerRemoved());
        processor.RegisterNestedType(() => new GameState());
    }
}