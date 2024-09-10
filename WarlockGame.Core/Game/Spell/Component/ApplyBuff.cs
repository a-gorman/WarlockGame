using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Buff;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Spell.Component;

class ApplyBuff : IWarlockComponent {
    private readonly int _radius;
    private readonly Func<Warlock,IBuff> _buffConstructor;
    public bool IgnoreCaster { get; init; } = false;
    
    public ApplyBuff(int radius, Func<Warlock,IBuff> buffConstructor) {
        _radius = radius;
        _buffConstructor = buffConstructor;
    }

    public void Invoke(Warlock caster, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            if(IgnoreCaster && target.Entity == caster) { continue; }
            if(target.Entity is Warlock warlock)
                warlock.AddBuff(_buffConstructor.Invoke(caster));
        }
    }
}