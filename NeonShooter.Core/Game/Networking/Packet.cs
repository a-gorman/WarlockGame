using LiteNetLib.Utils;

namespace NeonShooter.Core.Game.Networking; 

public class GameState: INetSerializable {
    // public 
    
    public void Serialize(NetDataWriter writer) {
        throw new System.NotImplementedException();
    }

    public void Deserialize(NetDataReader reader) {
        throw new System.NotImplementedException();
    }
}

public class Heartbeat {
    public uint Frame { get; set; }
    public uint Checksum { get; set; }
}