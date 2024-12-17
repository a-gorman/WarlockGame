using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Entity;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DamageComponent : IWarlockComponent {
    public required float Damage { get; init; }

    public float SelfFactor { get; init; } = 1;

    public void Invoke(Warlock caster, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            if (target.Entity is Warlock warlock) {
                var damageToInflict = Damage * target.FalloffFactor;
                if (warlock == caster) {
                    damageToInflict *= SelfFactor;
                }

                warlock.Damage(damageToInflict, caster);
            }
        }
    }
}