namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

/// <summary>
/// Causes the target entity to mirror the movements of the target entity
/// </summary>
class Shadow : Behavior {
    private readonly int _targetId;
    private readonly Simulation _simulation;

    private Vector2 _previousTargetPosition;

    public Shadow(int targetId, Simulation simulation) {
        _targetId = targetId;
        _simulation = simulation;
    }

    public override void OnAdd(Entity entity) {
        var target = getTarget();
        if (target is null) {
            IsExpired = true;
        }
        else {
            _previousTargetPosition = target.Position;
        }
    }

    public override void Update(Entity entity) {
        var target = getTarget();
        if (target is null) {
            IsExpired = true;
            return;
        }

        var displacement = target.Position - _previousTargetPosition;

        entity.Position += displacement;

        _previousTargetPosition = target.Position;
    }

    private Entity? getTarget() {
        return _simulation.EntityManager.GetEntity(_targetId);
    }
}