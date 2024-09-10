using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Spell.Component;

class PushComponent : IWarlockComponent {
    public required float Force { get; init; }
    public float SelfFactor { get; init; } = 1;
    public float ProjectileFactor { get; init; } = 0;
    public Func<Vector2, Vector2?, Vector2> DisplacementTransform { get; init; } = (x, _) => x;

    public void Invoke(Warlock caster, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            var forceToUse = Force * target.FalloffFactor;
            var direction = DisplacementTransform.Invoke(target.DisplacementAxis1, target.DisplacementAxis2);
            
            if (target.Entity is Warlock warlock) {
                if (warlock == caster) {
                    forceToUse *= SelfFactor;
                }

                warlock.Push(forceToUse, direction);
            } else if (target.Entity is Projectile projectile) {
                projectile.Push(forceToUse * ProjectileFactor, direction);
            }
        }
    }
}