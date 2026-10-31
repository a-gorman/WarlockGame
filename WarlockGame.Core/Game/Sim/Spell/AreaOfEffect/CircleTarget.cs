using System.Linq;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

class CircleTarget : ILocationShape {
    public bool IgnoreCaster { get; init; } = false;
    public bool IgnoreProjectiles { get; init; } = false;
    public float InnerRadius { get; }
    public float OuterRadius { get; }
    public Falloff.FalloffFactor FalloffFactor { get; init; } = Falloff.Linear;

    private readonly AoeSelector _targetSelector;

    public CircleTarget(AoeSelector targetSelector, int innerRadius, int outerRadius) {
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        _targetSelector = targetSelector;

        if(InnerRadius > OuterRadius) {
            Logger.Warning("Inner radius is greater than outer radius. Expanding outer radius to match.", Logger.LogType.Simulation);
            OuterRadius = InnerRadius;
        }
    }
    
    public CircleTarget(AoeSelector targetSelector, int radius): this(targetSelector, radius, radius) { }
    
    
    public AoeResult GatherTargets(SpellContext context, Vector2 origin) {
        if (Configuration.DebugAoeVisualize) {
            SimDebug.VisualizeCircle(InnerRadius, origin, Color.Blue, duration: 20);
            SimDebug.VisualizeCircle(OuterRadius, origin, Color.Yellow, duration: 20);
        }
        
        var targets = context.EntityManager.GetNearbyEntities(origin, OuterRadius)
                        .Where(x => _targetSelector.Invoke(x, context))
                        .Where(x => (!IgnoreCaster || x != context.Caster) && (!IgnoreProjectiles || x is not Projectile))
                        .Select(x => new AoeTargetInfo {
                            Entity = x,
                            OriginTargetDisplacement = x.Position - origin,
                            DisplacementAxis2 = x.Position - origin,
                            FalloffFactor = FalloffFactor.Invoke(x.Position - origin, OuterRadius, InnerRadius, x.Radius)
                        })
                        .ToList();

        return new AoeResult {
            Targets = targets,
            Center = origin,
            SoundRadius = OuterRadius
        };
    }
}

