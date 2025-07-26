using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class Friction: Behavior {
    private readonly float _a;
    private readonly float _b;
    private readonly float _c;

    // a*v^2 + b*v + c
    public Friction(float a, float b, float c) {
        _a = a;
        _b = b;
        _c = c;
    }

    public override void Update(Entity entity) {
        var frictionMagnitude = _a * entity.Velocity.LengthSquared() + _b * entity.Velocity.Length() + _c;

        if (frictionMagnitude.Squared() >= entity.Velocity.LengthSquared()) {
            entity.Velocity = Vector2.Zero;
            return;
        }

        entity.Velocity -= entity.Velocity.WithLength(frictionMagnitude);
    }
}