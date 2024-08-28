using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Geometry;
using WarlockGame.Core.Game.Graphics.Effect;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

class LineTarget : IDirectionalShape {

    public required int Length { get; init; }
    
    public IEffect? Animation { get; init; }
    
    public List<TargetInfo> GatherTargets(Warlock caster, Vector2 castLocation, Vector2 invokeDirection) {
        var startPoint = castLocation + caster.Radius * invokeDirection.ToNormalized();
        var endPoint = startPoint + invokeDirection * Length;

        var lineSegment = new LineSegment(startPoint, endPoint);

        return GatherTargets(lineSegment).ToList();
    }
        
    private IEnumerable<TargetInfo> GatherTargets(LineSegment lineSegment) {
        foreach (var entity in EntityManager.GetNearbyEntities(lineSegment.BoundingBox)) {
            var closetLinePoint = lineSegment.GetClosetPointTo(entity.Position);

            if (closetLinePoint.DistanceSquaredTo(entity.Position) > entity.Radius.Squared()) { continue; }

            var displacement = closetLinePoint - entity.Position;
            yield return new TargetInfo
            {
                Entity = entity,
                Displacement = displacement,
                FalloffFactor = 0 // How do we calculate this if we have non-zero width?
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