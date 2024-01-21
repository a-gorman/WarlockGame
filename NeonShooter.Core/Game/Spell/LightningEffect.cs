using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.Geometry;
using NeonShooter.Core.Game.Graphics.Effect;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Spell; 

public class LightningEffect: ICastEffect {

    private Texture2D _art = Art.Lightning;

    private const int Length = 800;
    
    public void OnCast(IEntity caster, Vector2 castDirection) {
        // DebugVisualize(caster, castDirection);

        var startPoint = caster.Position + caster.Radius * castDirection.ToNormalized();
        var endPoint = startPoint + castDirection * Length;

        var lineSegment = new LineSegment(startPoint, endPoint);

        foreach (var entity in EntityManager.GetNearbyEntities(lineSegment.BoundingBox)) {
            var closetPointTo = lineSegment.GetClosetPointTo(entity.Position);

            if (closetPointTo.DistanceSquaredTo(entity.Position) > entity.Radius.Squared()) { continue; }
            
            switch (entity)
            {
                case Warlock player:
                    if (ReferenceEquals(player, caster)) { break; }
                    
                    player.Push(50, player.Position - caster.Position);
                    player.Damage(30, caster);
                    break;
            }
        }
        
        EffectManager.Add(new Lightning(_art, startPoint, castDirection.ToAngle()));
    }

    /// <summary>
    /// For debugging
    /// </summary>
    private void DebugVisualize(IEntity caster, Vector2 castDirection) {
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