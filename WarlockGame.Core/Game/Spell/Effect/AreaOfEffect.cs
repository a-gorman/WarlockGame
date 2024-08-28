using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Spell.Effect;

class LocationAreaOfEffect: ILocationSpellEffect {

    public required ILocationShape Shape { get; init; } 
    public required IReadOnlyCollection<IWarlockEffect> Effects { get; init; }

    public void Invoke(Warlock caster, Vector2 invokeLocation) {
        foreach (var effect in Effects) {
            effect.Invoke(caster, Shape.GatherTargets(caster, invokeLocation));
        }
    }
}

class DirectionalAreaOfEffect: IDirectionalSpellEffect {

    public required IDirectionalShape Shape { get; init; } 
    public required IReadOnlyCollection<IWarlockEffect> Effects { get; init; }

    public void Invoke(Warlock caster, Vector2 castLocation, Vector2 invokeDirection) {
        foreach (var effect in Effects) {
            effect.Invoke(caster, Shape.GatherTargets(caster, castLocation, invokeDirection));
        }
    }
}

class SelfAreaOfEffect: ISelfSpellEffect {

    public required ILocationShape Shape { get; init; } 
    public required IReadOnlyCollection<IWarlockEffect> Effects { get; init; }

    public void Invoke(Warlock caster) {
        foreach (var effect in Effects) {
            effect.Invoke(caster, Shape.GatherTargets(caster, caster.Position));
        }
    }
}