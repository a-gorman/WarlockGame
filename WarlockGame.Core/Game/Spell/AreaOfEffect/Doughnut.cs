using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

/// <summary>
/// A circular shape that measures distance to the edge of the circle
/// </summary>
class Doughnut : ILocationShape {
    public required float Radius { get; init; }
    public required float Width { get; init; }
    public bool IgnoreCaster { get; init; } = false;
    public Texture2D? Texture { get; init; }
    public Falloff.FalloffFactor2Axis FalloffFactor { get; init; } = Falloff.Axis1Linear;

    public List<TargetInfo> GatherTargets(Warlock caster, Vector2 invokeLocation) {
        return EntityManager.GetNearbyEntities(invokeLocation, Radius + Width)
                            .Where(x => !IgnoreCaster || x != caster)
                            .Select(x => CreateTargetInfo(x, invokeLocation))
                            .ToList();
    }

    private TargetInfo CreateTargetInfo(EntityBase target, Vector2 invokeLocation) {
        var displacementAxis1 = target.Position - invokeLocation;
        
        var displacementAxis2 = Vector2.Zero;
        if (displacementAxis1.HasLength()) {
            // This will point towards the center of the doughnut rim
            displacementAxis2 = displacementAxis1.WithLength(Radius - displacementAxis1.Length()); 
        }

        return new TargetInfo
        {
            Entity = target,
            DisplacementAxis1 = displacementAxis1,
            DisplacementAxis2 = displacementAxis2,
            FalloffFactor = FalloffFactor.Invoke((displacementAxis1, Width), (displacementAxis2, Width), target.Radius)
        };
    }

}