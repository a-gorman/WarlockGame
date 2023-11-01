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

    public static void Visualize(Rectangle rectangle, Color color, int duration = 1) {
        EffectManager.Add(new VectorEffect(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Bottom), color, duration));
        EffectManager.Add(new VectorEffect(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Left, rectangle.Top), color, duration));
        EffectManager.Add(new VectorEffect(new Vector2(rectangle.Right, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Top), color, duration));
        EffectManager.Add(new VectorEffect(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Right, rectangle.Top), color, duration));
    }

    public static void Visualize(string input, Vector2 position) {
        NeonShooterGame.Instance.DrawDebugString(input, position);
    }
}