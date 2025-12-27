using System;
using System.Linq;
using WarlockGame.Core.Game.Sim.Effect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

/// <summary>
/// Creates new effects and adds them to the effect manager
/// </summary>
class EffectComponent : ILocationSpellComponent {
    public Func<SpellContext, Vector2, IEffect>[] EffectConstructors { get; private init; }
    
    public EffectComponent(params Func<SpellContext, Vector2, IEffect>[] effectConstructors) {
        EffectConstructors = effectConstructors;
    }
    
    public EffectComponent(params Func<Vector2, IEffect>[] effectConstructors) {
        EffectConstructors = effectConstructors
            .Select(func => new Func<SpellContext, Vector2, IEffect>((_, loc) => func.Invoke(loc)))
            .ToArray();
    }
    
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        foreach (var effect in EffectConstructors) {
            context.EffectManager.Add(effect.Invoke(context, invokeLocation));
        }
    }
}