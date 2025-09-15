using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class Yoyo : Behavior {
    private readonly Simulation _simulation;

    private State _state = State.Outwards;

    private readonly Vector2 _outwardsAccel;
    private readonly Vector2 _inwardsAccel;

    public Vector2 Velocity { get; set; } = Vector2.Zero;
    public GameTimer OutwardsTime { get; private set; }
    public GameTimer InwardsTime { get; private set; }

    public Yoyo(Simulation simulation, Vector2 maxDisplacement, SimTime outwardsTime, SimTime inwardsTime) {
        _simulation = simulation;
        OutwardsTime = outwardsTime.ToTimer();
        InwardsTime = inwardsTime.ToTimer();

        var maxDisplacementLength = maxDisplacement.Length();

        // dx = 1/2 a dt^2 + vi dt
        _outwardsAccel = maxDisplacement.WithLength(2f * maxDisplacementLength / outwardsTime.Ticks.Squared());
        _inwardsAccel = maxDisplacement.WithLength(2f * maxDisplacementLength / inwardsTime.Ticks.Squared());

        Velocity = -_outwardsAccel * outwardsTime.Ticks;
    }

    public override void Update(Entity entity) {
        if (_state == State.Outwards) {
            Velocity += _outwardsAccel;
            OutwardsTime = OutwardsTime.Decrement();
            if (OutwardsTime.IsExpired) {
                _state = State.Inwards;
            }
        }
        else {
            Velocity += _inwardsAccel;
            InwardsTime = InwardsTime.Decrement();
            if (InwardsTime.IsExpired) {
                IsExpired = true;
            }
        }

        entity.Position += Velocity;
    }

    private enum State {
        Outwards,
        Inwards
    }
}