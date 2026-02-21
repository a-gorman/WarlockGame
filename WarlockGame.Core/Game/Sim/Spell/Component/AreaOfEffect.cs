using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class LocationAreaOfEffect: ILocationSpellComponent {

    public required ILocationShape Shape { get; init; } 
    public required IReadOnlyCollection<IEntityComponent> Components { get; init; }

    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        var targetsHit = Shape.GatherTargets(context, invokeLocation);
        foreach (var effect in Components) {
            effect.Invoke(context, targetsHit);
        }
    }
}

class DirectionalAreaOfEffect: IDirectionalSpellComponent {

    public required IDirectionalShape Shape { get; init; } 
    public required IReadOnlyCollection<IEntityComponent> Effects { get; init; }

    public void Invoke(SpellContext context, Vector2 invokeLocation, Vector2 invokeDirection) {
        var entitiesHit = Shape.GatherTargets(context, invokeLocation, invokeDirection);
        foreach (var effect in Effects) {
            effect.Invoke(context, entitiesHit);
        }
    }
}

class SelfAreaOfEffect: ISelfSpellComponent {

    public required ILocationShape Shape { get; init; } 
    public required IReadOnlyCollection<IEntityComponent> Components { get; init; }

    public void Invoke(SpellContext context) {
        var entitiesHit = Shape.GatherTargets(context, context.Caster.Position);
        foreach (var effect in Components) {
            effect.Invoke(context, entitiesHit);
        }
    }
}