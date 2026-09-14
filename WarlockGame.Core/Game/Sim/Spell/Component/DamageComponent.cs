using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DamageComponent : IAoeTargetsSpellComponent {
    public required float Damage { get; init; }

    public float SelfFactor { get; init; } = 1;
    public bool Distributed { get; init; } = false;

    public void Invoke(SpellContext context, IReadOnlyCollection<AoeTargetInfo> targets) {
        foreach (var target in targets) {
            var damageToInflict = Damage * target.FalloffFactor * context.Caster.DamageMultiplier;

            if (target.Entity == context.Caster) {
                damageToInflict *= SelfFactor;
            }

            if (Distributed) {
                damageToInflict /= targets.Count;
            }

            target.Entity.Damage(damageToInflict, DamageType.Player, context.Caster);
        }
    }
}