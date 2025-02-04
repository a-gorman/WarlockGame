using System;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Effect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class EffectComponent : ILocationSpellComponent {

    public required Func<SpellContext, Vector2, IEffect> EffectConstructor { get; init; }
    
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        context.EffectManager.Add(EffectConstructor.Invoke(context, invokeLocation));
    }
}