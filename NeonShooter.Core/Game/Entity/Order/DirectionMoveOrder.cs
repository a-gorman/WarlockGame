using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Entity.Order;

class DirectionMoveOrder : IOrder {

    private readonly PlayerShip _player;
    private bool _active;

    public bool Finished { get; private set; }

    public DirectionMoveOrder(PlayerShip player) {
        _player = player;
    }

    public void Update() {
        var directionalInput = _player.Player.Input.GetDirectionalInput();

        if (directionalInput.HasLength()) {
            _player.Direction = directionalInput;
        }
        else {
            Finished = true;
        }
        
        _active = true;
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