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