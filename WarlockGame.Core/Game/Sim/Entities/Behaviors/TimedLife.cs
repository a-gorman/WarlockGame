namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class TimedLife : Behavior {
    private readonly GameTimer _timeToLive;

    public TimedLife(SimTimeSpan timeToLive) {
        _timeToLive = timeToLive.ToTimer();
    }

    public override void Update(Entity entity) {
        if (_timeToLive.Decrement()) {
            entity.IsExpired = true;
        }
    }
}