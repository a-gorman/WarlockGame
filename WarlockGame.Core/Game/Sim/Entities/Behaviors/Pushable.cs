namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

internal class Pushable : Behavior {

    public float Strength { get; set; } = 1.0f;

    private void Push(OnPushedEventArgs args) {
        args.Source.Velocity += args.Force * Strength;
    }
    
    public override void OnAdd(Entity entity) {
        entity.OnPushed += Push;
    }

    public override void OnRemove(Entity entity) {
        entity.OnPushed -= Push;
    }
}