using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Entity.Factory; 

static class WarlockFactory {
    public static Warlock FromPacket(Networking.Warlock warlock, int playerId) {
        return new Warlock(warlock.Id, playerId)
        {
            Position = warlock.Position,
            Velocity = warlock.Velocity,
            Orientation = warlock.Orientation
        };
    }

    public static Networking.Warlock ToPacket(Warlock warlock) {
        return new Networking.Warlock {
            Id = warlock.Id,
            Position = warlock.Position,
            Velocity = warlock.Velocity,
            Orientation = warlock.Orientation
        };
    }
}