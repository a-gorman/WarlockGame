using System;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

/// <summary>
/// Creates new effects and adds them to the effect manager
/// </summary>
class EffectComponent : ILocationSpellComponent {

    public required Func<SpellContext, Vector2, IEffect> EffectConstructor { get; init; }
    
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        context.EffectManager.Add(EffectConstructor.Invoke(context, invokeLocation));
    }
}