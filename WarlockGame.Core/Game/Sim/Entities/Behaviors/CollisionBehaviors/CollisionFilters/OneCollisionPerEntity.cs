using System.Collections.Generic;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors.CollisionFilters;

class OneCollisionPerEntity : Behavior {

    private readonly HashSet<int> _entitiesSeen = [];

    public override void OnAdd(Entity entity) {
        entity.OnCollision += AddToSeen;
        entity.CollisionFilters.Add(Filter);
    }

    public override void OnRemove(Entity entity) {
        entity.OnCollision -= AddToSeen;
        entity.CollisionFilters.Remove(Filter);
    }

    private bool Filter(Entity self, Entity other) {
        return !_entitiesSeen.Contains(other.Id);
    }

    private void AddToSeen(OnCollisionEventArgs args) {
        _entitiesSeen.Add(args.Other.Id);
    }
}