using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Geometry;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Effect.Display;
using WarlockGame.Core.Game.Util;
using SpriteEffect = WarlockGame.Core.Game.Sim.Effect.Display.SpriteEffect;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

class LineTarget : IDirectionalShape {
    public required int Length { get; init; }
    public int Width { get; init; } = 0;
    public bool IgnoreCaster { get; init; } = false;
    public Texture2D? Texture { get; init; }
    public Falloff.FalloffFactor2Axis FalloffFactor { get; init; } = Falloff.None;

    public List<TargetInfo> GatherTargets(SpellContext context, Vector2 castLocation, Vector2 invokeDirection) {
        var startPoint = castLocation + context.Caster.Radius * invokeDirection.ToNormalized();
        var endPoint = startPoint + invokeDirection * Length;

        // TODO: Make this scale in size
        Texture?.Run(x => {
            var sprite = new Sprite(x);
            context.EffectManager.Add(
                new SpriteEffect(sprite, castLocation, duration: SimTime.OfTicks(10), orientation: invokeDirection.ToAngle()) 
                {
                    Origin = new Vector2(0, sprite.Size.Y / 2)
                });
        });

        var lineSegment = new LineSegment(startPoint, endPoint);
        
        return GatherTargets(lineSegment, context).ToList();
    }
        
    private IEnumerable<TargetInfo> GatherTargets(LineSegment lineSegment, SpellContext context) {
        foreach (var entity in context.EntityManager.GetNearbyEntities(lineSegment.BoundingBox)) {
            if(IgnoreCaster && entity == context.Caster) { continue; }
            
            var closetLinePoint = lineSegment.GetClosetPointTo(entity.Position);

            if (closetLinePoint.DistanceSquaredTo(entity.Position) > entity.Radius.Squared()) { continue; }

            var displacement1 = entity.Position - lineSegment.Start;
            var displacement2 = entity.Position - closetLinePoint;
            yield return new TargetInfo
            {
                Entity = entity,
                OriginTargetDisplacement = displacement1,
                DisplacementAxis2 = displacement2,
                FalloffFactor = FalloffFactor.Invoke((displacement1, Length), (displacement2, Width), entity.Radius)
            };
        }
    }
    
    /// <summary>
    /// For debugging
    /// </summary>
    private void DebugVisualize(SpellContext context, Vector2 castDirection) {
        const int duration = 100;
        var caster = context.Caster;
        
        var startPoint = caster.Position + caster.Radius * castDirection.ToNormalized();
        var endPoint = startPoint + castDirection * Length;

        var lineSegment = new LineSegment(startPoint, endPoint);

        SimDebug.Visualize(lineSegment, Color.Red, duration);
        
        foreach (var entity in context.EntityManager.GetNearbyEntities(lineSegment.BoundingBox).Where(x => !ReferenceEquals(caster, x))) {
            var closetPointTo = lineSegment.GetClosetPointTo(entity.Position);

            SimDebug.VisualizeCircle(entity.Radius, entity.Position, Color.Cyan, duration);
            
            SimDebug.Visualize(new LineSegment(closetPointTo, entity.Position), Color.Yellow, duration);
            
            if (closetPointTo.DistanceSquaredTo(entity.Position) > entity.Radius.Squared()) { continue; }
        }
    }
}