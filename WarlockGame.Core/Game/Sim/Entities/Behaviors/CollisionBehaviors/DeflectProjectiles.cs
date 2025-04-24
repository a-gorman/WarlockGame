namespace WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors;

class DeflectProjectiles : Behavior {
    public override void OnAdd(Entity entity) {
        entity.OnCollision += OnCollision;
    }

    public override void OnRemove(Entity entity) {
        entity.OnCollision -= OnCollision;
    }

    private void OnCollision(OnCollisionEventArgs args) {
        if (args.Other is Projectile projectile) {
            projectile.Velocity *= -1;
        }
    }
}