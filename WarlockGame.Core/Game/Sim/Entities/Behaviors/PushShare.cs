namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class PushShare : Behavior {
    private readonly int _targetId;
    private readonly Simulation _simulation;

    public PushShare(int targetId, Simulation simulation) {
        _targetId = targetId;
        _simulation = simulation;
    }
    
    private void Push(OnPushedEventArgs args) {
        var target = _simulation.EntityManager.GetEntity(_targetId);
        if (target is not null) {
            target.Velocity += args.Force;
        }
    }
    
    public override void OnAdd(Entity entity) {
        entity.OnPushed += Push;
        base.OnAdd(entity);
    }

    public override void OnRemove(Entity entity) {
        entity.OnPushed -= Push;
    }
}