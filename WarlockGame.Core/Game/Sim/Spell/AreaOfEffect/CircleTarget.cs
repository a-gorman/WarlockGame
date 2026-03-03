using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

class CircleTarget : ILocationShape {
    public bool IgnoreCaster { get; init; } = false;
    public bool IgnoreProjectiles { get; init; } = false;
    public float InnerRadius { get; }
    public float OuterRadius { get; }
    public Texture2D? Texture { get; init; }
    public Falloff.FalloffFactor FalloffFactor { get; init; } = Falloff.Linear;

    public CircleTarget(int innerRadius = 0, int? outerRadius = null) {
        InnerRadius = innerRadius;
        OuterRadius = outerRadius ?? innerRadius;

        if(InnerRadius > OuterRadius) {
            Logger.Warning("Inner radius is greater than outer radius. Expanding outer radius to match.", Logger.LogType.Simulation);
            OuterRadius = InnerRadius;
        }
    }

    public List<TargetInfo> GatherTargets(SpellContext context, Vector2 origin) {
        return context.EntityManager.GetNearbyEntities(origin, OuterRadius)
                            .Where(x => (!IgnoreCaster || x != context.Caster) && (!IgnoreProjectiles || x is not Projectile))
                            .Select(x => new TargetInfo
                            {
                                Entity = x,
                                OriginTargetDisplacement = x.Position - origin,
                                DisplacementAxis2 = x.Position - origin,
                                FalloffFactor = FalloffFactor.Invoke(x.Position - origin, OuterRadius, InnerRadius, x.Radius)
                            })
                            .ToList();
    }

}

