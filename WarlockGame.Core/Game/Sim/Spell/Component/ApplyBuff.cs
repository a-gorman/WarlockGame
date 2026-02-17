using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class BuffComponent : IEntityComponent {
    private readonly Func<SpellContext,Buff>[] _buffConstructors;
    public bool IgnoreCaster { get; init; } = false;
    
    public BuffComponent(params Func<SpellContext,Buff>[] buffConstructors) {
        _buffConstructors = buffConstructors;
    }

    public void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            if(IgnoreCaster && target.Entity == context.Caster) { continue; }
            if (target.Entity is Warlock warlock) {
                foreach (var constructor in _buffConstructors) {
                    warlock.AddBuff(constructor.Invoke(context));
                }
            }
        }
    }
}