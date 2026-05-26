using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class LocationAreaOfEffect: ILocationSpellComponent {

    public required ILocationShape Shape { get; init; } 
    public required IReadOnlyCollection<IEntityComponent> Components { get; init; }
    public GameSound? Sound { get; init; }

    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        var aoeResult = Shape.GatherTargets(context, invokeLocation);
        foreach (var effect in Components) {
            effect.Invoke(context, aoeResult.Targets);
        }
        
        if (Sound != null) {
            Sound.Play(aoeResult.Center, aoeResult.SoundRadius);
        }
    }
}

class DirectionalAreaOfEffect: IDirectionalSpellComponent {

    public required IDirectionalShape Shape { get; init; } 
    public required IReadOnlyCollection<IEntityComponent> Effects { get; init; }
    public GameSound? Sound { get; init; }

    public void Invoke(SpellContext context, Vector2 invokeLocation, Vector2 invokeDirection) {
        var aoeResult = Shape.GatherTargets(context, invokeLocation, invokeDirection);
        foreach (var effect in Effects) {
            effect.Invoke(context, aoeResult.Targets);
        }
        
        if (Sound != null) {
            Sound.Play(aoeResult.Center, aoeResult.SoundRadius);
        }
    }
}

class SelfAreaOfEffect: ISelfSpellComponent {

    public required ILocationShape Shape { get; init; } 
    public required IReadOnlyCollection<IEntityComponent> Components { get; init; }
    public GameSound? Sound { get; init; }

    public void Invoke(SpellContext context) {
        var aoeResult = Shape.GatherTargets(context, context.Caster.Position);
        foreach (var effect in Components) {
            effect.Invoke(context, aoeResult.Targets);
        }

        if (Sound != null) {
            Sound.Play(aoeResult.Center, aoeResult.SoundRadius);
        }
    }
}