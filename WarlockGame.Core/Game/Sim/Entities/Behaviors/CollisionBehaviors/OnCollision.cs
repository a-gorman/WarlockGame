using System;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors;

class OnCollision: Behavior {
    private readonly Action<OnCollisionEventArgs> _onCollision;
    
    public OnCollision(Action<OnCollisionEventArgs> onCollision) {
        _onCollision = onCollision;
    }

    public virtual void OnAdd(Entity entity) {
        entity.OnCollision += _onCollision;
    }

    public virtual void OnRemove(Entity entity) {
        entity.OnCollision -= _onCollision;
    }
}