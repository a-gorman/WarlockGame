using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Entity.Order;

class DirectionMoveOrder : IOrder {

    private readonly Warlock _player;
    private bool _active;

    public bool Finished { get; private set; }

    public DirectionMoveOrder(Warlock player) {
        _player = player;
    }

    public void Update() {
        // var directionalInput = _player.Player.Input.GetDirectionalInput();
        // if (directionalInput.HasValue) {
        //     _player.Direction = directionalInput.Value.HasLength() ? directionalInput : null;
        // }
        // else {
        //     Finished = true;
        // }
        //
        // _active = true;
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