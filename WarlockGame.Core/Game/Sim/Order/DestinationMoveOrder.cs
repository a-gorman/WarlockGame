using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Order; 

class DestinationMoveOrder : IOrder {

    private readonly Warlock _player;
    private readonly Vector2 _destination;
    private bool _active;

    public bool Finished { get; private set; } = false;
    
    public DestinationMoveOrder(Vector2 destination, Warlock player) {
        _destination = destination;
        _player = player;
    }

    public void Update() {
        _active = true;
        
        if (_player.Position.DistanceSquaredTo(_destination) < Warlock.Speed.Squared()) {
            Finished = true;
        }
        else {
            _player.Direction = (_destination - _player.Position).ToNormalizedOrZero();
        }
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