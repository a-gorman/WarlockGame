using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Entity.Order; 

class DestinationMoveOrder : IOrder {

    private readonly PlayerShip _player;
    private readonly Vector2 _destination;
    private bool _active;

    public bool Finished { get; private set; } = false;
    
    public DestinationMoveOrder(Vector2 destination, PlayerShip player) {
        _destination = destination;
        _player = player;
    }

    public bool Update() {
        _active = true;
        
        if (_player.Position.DistanceSquaredTo(_destination) < PlayerShip.Speed.Squared()) {
            Finished = true;
            return true;
        }

        _player.Direction = (_destination - _player.Position).ToNormalizedOrZero();
        return false;
    }

    public void OnCancel() {
        if (_active) {
            _player.Direction = null;
        }
    }

    public void OnFinish() {
        _player.Direction = null;
    }
}