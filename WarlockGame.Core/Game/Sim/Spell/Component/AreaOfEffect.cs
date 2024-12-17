using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Entity;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class LocationAreaOfEffect: ILocationSpellComponent {

    public required ILocationShape Shape { get; init; } 
    public required IReadOnlyCollection<IWarlockComponent> Effects { get; init; }

    public void Invoke(Warlock caster, Vector2 invokeLocation) {
        foreach (var effect in Effects) {
            effect.Invoke(caster, Shape.GatherTargets(caster, invokeLocation));
        }
    }
}

class DirectionalAreaOfEffect: IDirectionalSpellComponent {

    public required IDirectionalShape Shape { get; init; } 
    public required IReadOnlyCollection<IWarlockComponent> Effects { get; init; }

    public void Invoke(Warlock caster, Vector2 castLocation, Vector2 invokeDirection) {
        foreach (var effect in Effects) {
            effect.Invoke(caster, Shape.GatherTargets(caster, castLocation, invokeDirection));
        }
    }
}

class SelfAreaOfEffect: ISelfSpellComponent {

    public required ILocationShape Shape { get; init; } 
    public required IReadOnlyCollection<IWarlockComponent> Components { get; init; }

    public void Invoke(Warlock caster) {
        foreach (var effect in Components) {
            effect.Invoke(caster, Shape.GatherTargets(caster, caster.Position));
        }
    }
}