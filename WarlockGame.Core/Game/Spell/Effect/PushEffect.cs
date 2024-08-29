using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Spell.Effect;

class PushEffect : IWarlockEffect {
    public required int Force { get; init; }
    public float SelfFactor { get; init; } = 1;
    public Func<Vector2, Vector2?, Vector2> DisplacementTransform { get; init; } = (x, _) => x;

    public void Invoke(Warlock caster, IReadOnlyCollection<TargetInfo> targets) {
        foreach (var target in targets) {
            if (target.Entity is Warlock warlock) {
                var forceToUse = Force * target.FalloffFactor;
                if (warlock == caster) {
                    forceToUse *= SelfFactor;
                }

                warlock.Push(forceToUse, DisplacementTransform.Invoke(target.DisplacementAxis1, target.DisplacementAxis2));
            }
        }
    }
}