using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Entity.Order; 

class MoveOrder : IOrder {

    private readonly Vector2 _destination;

    public MoveOrder(Vector2 destination) {
        _destination = destination;
    }

    public bool Update(PlayerShip player) {
        if (player.Position.DistanceSquaredTo(_destination) < PlayerShip.Speed) {
            player.Position = _destination;
            player.Direction = null;
            return true;
        }

        player.Direction = (_destination - player.Position).ToNormalizedOrZero();
        return false;
    }
}