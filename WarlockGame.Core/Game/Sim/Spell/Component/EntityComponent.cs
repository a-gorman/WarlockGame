using System;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

/// <summary>
/// Creates a new entity and adds it to the entity manager
/// </summary>
class EntityComponent: ILocationSpellComponent {
    public required Func<SpellContext, Vector2, Entity> EntityConstructor { get; init; }
        
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        var entity = EntityConstructor.Invoke(context, invokeLocation);
        context.EntityManager.Add(entity);
    }
}