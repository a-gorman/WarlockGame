using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Entity.Order;

class DirectionMoveOrder : IOrder {

    private readonly PlayerShip _player;
    private readonly Vector2 _direction;
    private bool _active;

    // Lasts only a single frame
    public bool Finished => true;

    public DirectionMoveOrder(Vector2 direction, PlayerShip player) {
        _player = player;
        _direction = direction.ToNormalized();
    }

    public bool Update() {
        _player.Direction = _direction;
        _active = true;
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