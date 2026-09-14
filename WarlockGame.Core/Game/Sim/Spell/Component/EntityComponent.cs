using System;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

/// <summary>
/// Creates a new entity and adds it to the entity manager
/// </summary>
class EntityComponent: IDirectionalSpellComponent {
    public Func<SpellContext, Vector2, Vector2, Entity> EntityConstructor { get; init; }
    
    
    public EntityComponent(Func<SpellContext, Vector2, Vector2, Entity> entityConstructor) {
        EntityConstructor = entityConstructor;
    }

    public void Invoke(SpellContext context, Vector2 invokeLocation, Vector2 invokeDirection) {
        var entity = EntityConstructor.Invoke(context, invokeLocation, invokeDirection);
        context.EntityManager.Add(entity);
    }
}