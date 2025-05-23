using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Geometry;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Effect.Display;

namespace WarlockGame.Core.Game.Sim; 

public static class SimDebug {
    private static EffectManager? EffectManager => WarlockGame.Instance.Simulation.EffectManager;
    
    public static void Visualize(Vector2 vectorToVisualize, Vector2 position, Color color, int duration = 1) {
        EffectManager?.Add(new VectorEffect(position, vectorToVisualize + position, color, duration));
    }
    
    public static void Visualize(LineSegment lineSegment, Color color, int duration = 1) {
        EffectManager?.Add(new VectorEffect(lineSegment.Start, lineSegment.End, color, duration));
    }

    public static void Visualize(Rectangle rectangle, Color color, int duration = 1) {
        EffectManager?.Add(new VectorEffect(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Bottom), color, duration));
        EffectManager?.Add(new VectorEffect(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Left, rectangle.Top), color, duration));
        EffectManager?.Add(new VectorEffect(new Vector2(rectangle.Right, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Top), color, duration));
        EffectManager?.Add(new VectorEffect(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Right, rectangle.Top), color, duration));
    }
    
    public static void VisualizeCircle(float radius, Vector2 position, Color color, int duration = 1) {
        EffectManager?.Add(new CircleEffect(radius, position, color, duration));
    }
    
    public static void VisualizePoint(Vector2 position, Color color, int duration = 1) {
        EffectManager?.Add(new PointEffect(position, color, duration));
    }

    public static void Visualize(string input, Vector2 position, int duration = 1) {
        EffectManager?.Add(new StringEffect(input, position, Color.White, duration));
    }
    
    public static void Visualize(OrientedRectangle rectangle, Color color, int duration = 1) {
        EffectManager?.Add(new PolygonEffect(rectangle, color, duration));
    }
    
    public static void Visualize(IEnumerable<string> input, Vector2 position, int duration = 1) {
        var offset = Vector2.Zero;
        foreach (var s in input) {
            EffectManager?.Add(new StringEffect(s, position + offset, Color.White, duration));
            offset.Y += 23;
        }
    }
}