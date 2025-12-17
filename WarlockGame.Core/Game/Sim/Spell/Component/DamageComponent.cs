using System.Collections.Generic;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DamageComponent : ITargetComponent {
    public required float Damage { get; init; }

    public float SelfFactor { get; init; } = 1;

    public void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            var damageToInflict = Damage * target.FalloffFactor * context.Caster.DamageMultiplier;
            if (target.Entity == context.Caster) {
                damageToInflict *= SelfFactor;
            }

            target.Entity.Damage(damageToInflict, context.Caster);
            
            Logger.Debug($"Dealt {damageToInflict} damage!", Logger.LogType.Simulation);
        }
    }
}