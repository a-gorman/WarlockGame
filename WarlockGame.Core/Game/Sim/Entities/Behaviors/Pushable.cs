namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

internal class Pushable : Behavior {

    private void Push(OnPushedEventArgs args) {
        args.Source.Velocity += args.Force;
    }
    
    public override void OnAdd(Entity entity) {
        entity.OnPushed += Push;
        base.OnAdd(entity);
    }

    public override void OnRemove(Entity entity) {
        entity.OnPushed -= Push;
    }
}