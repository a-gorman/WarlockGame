using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Geometry;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Graphics.Effect;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

class LineTarget : IDirectionalShape {
    public required int Length { get; init; }
    public int Width { get; init; } = 0;
    public bool IgnoreCaster { get; init; } = false;
    public Texture2D? Texture { get; init; }
    public Falloff.FalloffFactor2Axis FalloffFactor { get; init; } = Falloff.None;

public List<TargetInfo> GatherTargets(Warlock caster, Vector2 castLocation, Vector2 invokeDirection) {
        var startPoint = castLocation + caster.Radius * invokeDirection.ToNormalized();
        var endPoint = startPoint + invokeDirection * Length;
        
        // TODO: Make this scale in size
        Texture?.Run(x => EffectManager.Add(new Lightning(x, castLocation, invokeDirection.ToAngle())));

        var lineSegment = new LineSegment(startPoint, endPoint);
        
        return GatherTargets(lineSegment, caster).ToList();
    }
        
    private IEnumerable<TargetInfo> GatherTargets(LineSegment lineSegment, Warlock caster) {
        foreach (var entity in EntityManager.GetNearbyEntities(lineSegment.BoundingBox)) {
            if(IgnoreCaster && entity == caster) { continue; }
            
            var closetLinePoint = lineSegment.GetClosetPointTo(entity.Position);

            if (closetLinePoint.DistanceSquaredTo(entity.Position) > entity.Radius.Squared()) { continue; }

            var displacement1 = entity.Position - lineSegment.Start;
            var displacement2 = entity.Position - closetLinePoint;
            yield return new TargetInfo
            {
                Entity = entity,
                DisplacementAxis1 = displacement1,
                DisplacementAxis2 = displacement2,
                FalloffFactor = FalloffFactor.Invoke((displacement1, Length), (displacement2, Width), entity.Radius)
            };
        }
    }
    
    /// <summary>
    /// For debugging
    /// </summary>
    private void DebugVisualize(Warlock caster, Vector2 castDirection) {
        int duration = 100;
        
        var startPoint = caster.Position + caster.Radius * castDirection.ToNormalized();
        var endPoint = startPoint + castDirection * Length;

        var lineSegment = new LineSegment(startPoint, endPoint);

        Debug.Visualize(lineSegment, Color.Red, duration);
        
        foreach (var entity in EntityManager.GetNearbyEntities(lineSegment.BoundingBox).Where(x => !ReferenceEquals(caster, x))) {
            var closetPointTo = lineSegment.GetClosetPointTo(entity.Position);

            Debug.VisualizeCircle(entity.Radius, entity.Position, Color.Cyan, duration);
            
            Debug.Visualize(new LineSegment(closetPointTo, entity.Position), Color.Yellow, duration);
            
            if (closetPointTo.DistanceSquaredTo(entity.Position) > entity.Radius.Squared()) { continue; }
        }
    }
}