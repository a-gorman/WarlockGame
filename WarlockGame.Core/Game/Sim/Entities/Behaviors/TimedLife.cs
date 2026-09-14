using System;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class TimedLife : Behavior {
    private GameTimer _timeToLive;

    public TimedLife(SimTime timeToLive) {
        _timeToLive = timeToLive.ToTimer();
    }

    public override void Update(Entity entity) {
        _timeToLive = _timeToLive.Decremented();
        if (_timeToLive.IsExpired) {
            entity.IsExpired = true;
        }
    }
}

class TimedLife<T> : Behavior where T: Entity {
    private GameTimer _timeToLive;
    private readonly Action<T> _onExpiration;

    public TimedLife(SimTime timeToLive, Action<T> onExpiration) {
        _timeToLive = timeToLive.ToTimer();
        _onExpiration = onExpiration;
    }

    public override void Update(Entity entity) {
        _timeToLive = _timeToLive.Decremented();
        if (_timeToLive.IsExpired) {
            entity.IsExpired = true;
            _onExpiration.Invoke((entity as T)!);
        }
    }
}