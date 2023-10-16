using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Geometry;
using NeonShooter.Core.Game.Graphics.Effect;

namespace NeonShooter.Core.Game.Util; 

public class Debug {
    public static void Visualize(Vector2 vectorToVisualize, Vector2 position, Color color, int duration = 1) {
        
        EffectManager.Add(new VectorEffect(position, vectorToVisualize + position, color, duration));
    }
    
    public static void Visualize(LineSegment lineSegment, Color color, int duration = 1) {
        
        EffectManager.Add(new VectorEffect(lineSegment.Start, lineSegment.End, color, duration));
    }
}