using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DeflectProjectile : IAoeTargetsSpellComponent {
    public void Invoke(SpellContext context, IReadOnlyCollection<AoeTargetInfo> targets) {
        foreach (var target in targets) {
            if (target.Entity is Projectile projectile) {
                projectile.Velocity = -1 * projectile.Velocity;
            }
        }
    } 
}