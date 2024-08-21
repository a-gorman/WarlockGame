using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Geometry;
using WarlockGame.Core.Game.Graphics.Effect;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Spell.Effect; 

public class LightningEffect: IDirectionalSpellEffect {

    private readonly Texture2D _art = Art.Lightning;

    private const int Length = 800;
    
    public void Invoke(IEntity caster, Vector2 castLocation, Vector2 castDirection) {
        // DebugVisualize(caster, castDirection);

        var startPoint = castLocation + caster.Radius * castDirection.ToNormalized();
        var endPoint = startPoint + castDirection * Length;

        var lineSegment = new LineSegment(startPoint, endPoint);

        foreach (var entity in EntityManager.GetNearbyEntities(lineSegment.BoundingBox)) {
            var closetPointTo = lineSegment.GetClosetPointTo(entity.Position);

            if (closetPointTo.DistanceSquaredTo(entity.Position) > entity.Radius.Squared()) { continue; }
            
            switch (entity)
            {
                case Warlock player:
                    if (ReferenceEquals(player, caster)) { break; }
                    
                    player.Push(50, player.Position - castLocation);
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