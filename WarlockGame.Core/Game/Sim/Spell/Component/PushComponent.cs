using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class PushComponent : IEntityComponent {
    public required float Force { get; init; }
    public float SelfFactor { get; init; } = 1;
    public float ProjectileFactor { get; init; } = 0;
    public Func<Vector2, Vector2, Vector2> DisplacementTransform { get; init; } = (x, _) => x;

    public void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            var forceFactor = 1f;
            if (target.Entity == context.Caster) {
                if(SelfFactor == 0) continue;
                forceFactor = SelfFactor;
            }
            
            if (target.Entity is Projectile) {
                if (ProjectileFactor == 0) {
                    continue;
                }
                
                forceFactor = ProjectileFactor;
            }
            
            var forceToUse = Force * target.FalloffFactor;
            var direction = DisplacementTransform.Invoke(target.OriginTargetDisplacement, target.DisplacementAxis2);

            target.Entity.Push(direction.WithLength(forceToUse * forceFactor));
        }
    }
}