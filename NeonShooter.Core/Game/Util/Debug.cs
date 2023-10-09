using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Graphics.Effect;

namespace NeonShooter.Core.Game.Util; 

public class Debug {
    public static void Visualize(Vector2 vectorToVisualize, Vector2 position, Color color, int duration = 1) {
        
        EffectManager.Add(new VectorEffect(position, vectorToVisualize + position, color, duration));
    }
}