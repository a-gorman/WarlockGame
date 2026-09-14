using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

/// <summary>
/// Creates new effects at a location and adds them to the effect manager
/// </summary>
class LocationEffectComponent : ILocationSpellComponent, IAoeTargetsSpellComponent, IEntitySpellComponent, ISelfSpellComponent {
    public Func<SpellContext, Vector2, IEffect>[] EffectConstructors { get; private init; }
    
    public LocationEffectComponent(params Func<SpellContext, Vector2, IEffect>[] effectConstructors) {
        EffectConstructors = effectConstructors;
    }
    
    public LocationEffectComponent(params Func<Vector2, IEffect>[] effectConstructors) {
        EffectConstructors = effectConstructors
            .Select(func => new Func<SpellContext, Vector2, IEffect>((_, loc) => func.Invoke(loc)))
            .ToArray();
    }
    
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        foreach (var effect in EffectConstructors) {
            context.EffectManager.Add(effect.Invoke(context, invokeLocation));
        }
    }

    public void Invoke(SpellContext context, IReadOnlyCollection<AoeTargetInfo> targets) {
        foreach (var target in targets) {
            foreach (var effect in EffectConstructors) {
                context.EffectManager.Add(effect.Invoke(context, target.Entity.Position));
            }
        }
    }

    public void Invoke(SpellContext context) {
        Invoke(context, context.Caster);
    }

    public void Invoke(SpellContext context, Entity target) {
        foreach (var effect in EffectConstructors) {
            context.EffectManager.Add(effect.Invoke(context, target.Position));
        }
    }
}

/// <summary>
/// Creates new effects at a location with a direction and adds them to the effect manager
/// </summary>
class DirectionalEffectComponent : IDirectionalSpellComponent {
    public Func<SpellContext, Vector2, Vector2, IEffect>[] EffectConstructors { get; private init; }
    
    public DirectionalEffectComponent(params Func<SpellContext, Vector2, Vector2, IEffect>[] effectConstructors) {
        EffectConstructors = effectConstructors;
    }
    
    public DirectionalEffectComponent(params Func<Vector2, Vector2, IEffect>[] effectConstructors) {
        EffectConstructors = effectConstructors
            .Select(func => new Func<SpellContext, Vector2, Vector2, IEffect>((_, loc, direction) => func.Invoke(loc, direction)))
            .ToArray();
    }
    
    public void Invoke(SpellContext context, Vector2 invokeLocation, Vector2 invokeDirection) {
        foreach (var effect in EffectConstructors) {
            context.EffectManager.Add(effect.Invoke(context, invokeLocation, invokeDirection));
        }
    }
}