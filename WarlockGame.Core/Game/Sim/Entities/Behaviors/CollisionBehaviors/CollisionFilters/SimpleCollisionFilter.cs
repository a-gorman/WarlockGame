using System.Collections.Generic;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors.CollisionFilters;

/// <summary>
/// Adds simple, stateless collision filters
/// </summary>
class SimpleCollisionFilter(params List<CollisionFilter> filters) : Behavior {
    public override void OnAdd(Entity entity) {
        entity.CollisionFilters.AddRange(filters);
    }

    public override void OnRemove(Entity entity) {
        foreach (var collisionFilter in filters) {
            entity.CollisionFilters.Remove(collisionFilter);
        }
    }

    public static bool IgnoreFriendlies(Entity source, Entity other) => source.PlayerId != other.PlayerId;
}