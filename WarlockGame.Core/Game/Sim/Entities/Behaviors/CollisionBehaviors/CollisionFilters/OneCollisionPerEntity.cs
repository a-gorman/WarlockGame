using System.Collections.Generic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors.CollisionFilters;

class OneCollisionPerEntity : Behavior {

    public SimTime? Cooldown { get; set; }
    
    private readonly Dictionary<int, GameTimer?> _entitiesSeen = [];

    public override void OnAdd(Entity entity) {
        entity.OnCollision += AddToSeen;
        entity.CollisionFilters.Add(Filter);
    }

    public override void OnRemove(Entity entity) {
        entity.OnCollision -= AddToSeen;
        entity.CollisionFilters.Remove(Filter);
    }

    public override void Update(Entity entity) {
        if (Cooldown != null) {
            foreach (var entitySeen in _entitiesSeen) {
                var newTimer = entitySeen.Value!.Value.Decremented();
                _entitiesSeen[entitySeen.Key] = newTimer;
            }
            
            _entitiesSeen.RemoveAll((key, value) => value!.Value.IsExpired);
        }
    }

    private bool Filter(Entity self, Entity other) {
        return !_entitiesSeen.ContainsKey(other.Id);
    }

    private void AddToSeen(OnCollisionEventArgs args) {
        _entitiesSeen.Add(args.Other.Id, Cooldown?.Let(x => x.ToTimer()));
    }
}