using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

static class Falloff {
    
    public delegate float FalloffFactor(Vector2 displacement, float maxDisplacement, float targetRadius);
    public delegate float FalloffFactor2Axis((Vector2 displacement, float max) axis1, (Vector2 displacement, float max) axis2, float targetRadius);
    
    public static float Linear(Vector2 displacement, float maxDisplacement, float targetRadius) {
        if (maxDisplacement == 0) return 1;
        
        return 1 - float.Clamp((displacement.Length() - targetRadius) / maxDisplacement, 0, 1);
    }
    
    public static float None(Vector2 displacement, float maxDisplacement, float targetRadius) {
        return 1;
    }
    
    public static float Axis1Linear((Vector2 displacement, float max) axis1, (Vector2 displacement, float max) _, float targetRadius) {
        return Linear(axis1.displacement, axis1.max, targetRadius);
    }
    
    public static float None((Vector2 displacement, float max) axis1, (Vector2 displacement, float max) axis2, float targetRadius) {
        return 1;
    }
}