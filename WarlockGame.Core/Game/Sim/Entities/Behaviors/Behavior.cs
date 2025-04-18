namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

abstract class Behavior
{
    public bool IsExpired { get; set; }
    public virtual void OnAdd(Entity entity) { }
    public virtual void Update(Entity entity) { }
    public virtual void OnRemove(Entity entity) { }
}