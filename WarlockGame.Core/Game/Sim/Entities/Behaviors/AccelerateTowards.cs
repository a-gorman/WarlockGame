using System;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class AccelerateTowards: Behavior {
    private readonly float _acceleration;
    private readonly Func<Entity, Vector2?> _targetLocation;

    public AccelerateTowards(float acceleration, Func<Entity, Vector2?> targetLocation) {
        _acceleration = acceleration;
        _targetLocation = targetLocation;
    }

    public override void Update(Entity entity) {
        var location = _targetLocation.Invoke(entity);
        if (location is not null) {
            entity.Velocity += (location.Value - entity.Position).WithLength(_acceleration);
        }
    }
}