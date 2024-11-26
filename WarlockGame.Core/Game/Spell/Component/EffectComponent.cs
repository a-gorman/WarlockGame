using System;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Effect;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.Component;

class EffectComponent : ILocationSpellComponent {

    public required Func<Warlock, Vector2, IEffect> EffectConstructor { get; init; }
    
    public void Invoke(Warlock caster, Vector2 invokeLocation) {
        EffectManager.Add(EffectConstructor.Invoke(caster, invokeLocation));
    }
}