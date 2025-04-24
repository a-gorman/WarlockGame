using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Buff;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class ApplyBuff : ITargetComponent {
    private readonly int _radius;
    private readonly Func<SpellContext,IBuff> _buffConstructor;
    public bool IgnoreCaster { get; init; } = false;
    
    public ApplyBuff(int radius, Func<SpellContext,IBuff> buffConstructor) {
        _radius = radius;
        _buffConstructor = buffConstructor;
    }

    public void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            if(IgnoreCaster && target.Entity == context.Caster) { continue; }
            if(target.Entity is Warlock warlock)
                warlock.AddBuff(_buffConstructor.Invoke(context));
        }
    }
}