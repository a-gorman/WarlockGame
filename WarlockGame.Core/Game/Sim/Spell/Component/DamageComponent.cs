using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DamageComponent : IEntityComponent {
    public required float Damage { get; init; }

    public float SelfFactor { get; init; } = 1;
    public bool Distributed { get; init; } = false;

    public void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            var damageToInflict = Damage * target.FalloffFactor * context.Caster.DamageMultiplier;
            if (Distributed) {
                damageToInflict = targets.Count;
            }

            if (target.Entity == context.Caster) {
                damageToInflict *= SelfFactor;
            }

            target.Entity.Damage(damageToInflict, DamageType.Player, context.Caster);
        }
    }
}