namespace WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors;

class StealProjectiles : Behavior {

    private readonly int _ownerId;
    private readonly Simulation _sim;

    public StealProjectiles(int ownerId, Simulation sim) {
        _ownerId = ownerId;
        _sim = sim;
    }

    public override void OnAdd(Entity entity) {
        entity.OnCollision += OnCollision;
    }

    public override void OnRemove(Entity entity) {
        entity.OnCollision -= OnCollision;
    }

    private void OnCollision(OnCollisionEventArgs args) {
        if (args.Other is Projectile projectile) {
            var owner = _sim.EntityManager.GetWarlockByForceId(_ownerId);
            if (owner != null) {
                projectile.Context.Caster = owner;
            }
        }
    }
}