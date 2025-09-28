using System.Collections.Generic;
using LiteNetLib.Utils;

namespace WarlockGame.Core.Game.Networking;

public static class SerializingExtensions {
    public static void Put(this NetDataWriter writer, Vector2 vector) {
        writer.Put(vector.X);
        writer.Put(vector.Y);
    }

    public static Vector2 GetVector2(this NetDataReader reader) {
        return new Vector2(reader.GetFloat(), reader.GetFloat());
    }

    public static T Get<T>(this NetDataReader reader) where T : INetSerializable, new() {
        var packet = new T();
        packet.Deserialize(reader);
        return packet;
    }

    public static void PutMany(this NetDataWriter writer,
        IReadOnlyCollection<INetSerializable> serializableCollection) {
        writer.Put(serializableCollection.Count);
        foreach (var serializable in serializableCollection) {
            serializable.Serialize(writer);
        }
    }

    public static List<T> GetMany<T>(this NetDataReader reader) where T : INetSerializable, new() {
        var count = reader.GetInt();
        var list = new List<T>(count);
        for (int i = 0; i < count; i++) {
            var item = new T();
            item.Deserialize(reader);
            list.Add(item);
        }

        return list;
    }
}