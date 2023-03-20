using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Util;

public static class VectorExtensions
{
    public static bool IsZeroVector(this Vector2 vector)
    {
        return vector.LengthSquared() == 0;
    }
    
    public static bool HasLength(this Vector2 vector)
    {
        return vector.LengthSquared() >= 0;
    }
}