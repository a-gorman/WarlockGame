using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Networking; 

public static class NetworkExtensions {
    public static void RegisterWarlockNestedTypes(this NetPacketProcessor processor) {
        processor.RegisterNestedType<Vector2>((w, v) => w.Put(v), reader => reader.GetVector2());
        processor.RegisterNestedType<Warlock>(() => new Warlock());
        processor.RegisterNestedType<Player>(() => new Player());
        processor.RegisterNestedType<MoveCommand>(() => new MoveCommand());
        processor.RegisterNestedType<CastCommand>(() => new CastCommand());
    }
}