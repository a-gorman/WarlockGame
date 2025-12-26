using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

/// <summary>
/// A circular shape that measures distance to the edge of the circle
/// </summary>
class Doughnut : ILocationShape {
    public required float Radius { get; init; }
    public required float Width { get; init; }
    public bool IgnoreCaster { get; init; } = false;
    public Texture2D? Texture { get; init; }
    public Falloff.FalloffFactor2Axis FalloffFactor { get; init; } = Falloff.Axis1Linear;

    public List<TargetInfo> GatherTargets(SpellContext context, Vector2 invokeLocation) {
        
        SimDebug.VisualizeCircle(Radius, invokeLocation, Color.Bisque, 5);
        
        return context.EntityManager.GetNearbyEntities(invokeLocation, Radius + Width)
                            .Where(x => !IgnoreCaster || x != context.Caster)
                            .Select(x => CreateTargetInfo(x, invokeLocation))
                            .Where(x => x.DisplacementAxis2.IsLengthLessThan(Width))
                            .ToList();
    }

    private TargetInfo CreateTargetInfo(Entity target, Vector2 invokeLocation) {
        var displacementAxis1 = target.Position - invokeLocation;
        
        var displacementAxis2 = Vector2.Zero;
        if (displacementAxis1.HasLength()) {
            // This will point towards the center of the doughnut rim
            displacementAxis2 = displacementAxis1.WithLength(Radius - displacementAxis1.Length()); 
        }

        return new TargetInfo
        {
            Entity = target,
            OriginTargetDisplacement = displacementAxis1,
            DisplacementAxis2 = displacementAxis2,
            FalloffFactor = FalloffFactor.Invoke((displacementAxis1, Width), (displacementAxis2, Width), target.Radius)
        };
    }

}