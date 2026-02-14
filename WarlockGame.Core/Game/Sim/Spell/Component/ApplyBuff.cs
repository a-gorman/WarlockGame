using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class BuffComponent : IEntityComponent {
    private readonly Func<SpellContext,Buff> _buffConstructor;
    public bool IgnoreCaster { get; init; } = false;
    
    public BuffComponent(Func<SpellContext,Buff> buffConstructor) {
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