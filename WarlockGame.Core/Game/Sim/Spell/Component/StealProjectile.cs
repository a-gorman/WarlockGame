using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class StealProjectile : IEntityComponent {
    public void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            if (target.Entity is Projectile projectile) {
                projectile.Context.Caster = context.Caster;
            }
        }
    }
}