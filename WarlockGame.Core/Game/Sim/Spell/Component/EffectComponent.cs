using System;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Entity;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class EffectComponent : ILocationSpellComponent {

    public required Func<Warlock, Vector2, IEffect> EffectConstructor { get; init; }
    
    public void Invoke(Warlock caster, Vector2 invokeLocation) {
        EffectManager.Add(EffectConstructor.Invoke(caster, invokeLocation));
    }
}