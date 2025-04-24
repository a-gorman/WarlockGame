using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace WarlockGame.Core.Game.Util;

public static class VectorExtensions
{
    public static Vector2 NanToZero(this Vector2 source) {
        source.X = source.X.IsNan() ? 0 : source.X;
        source.Y = source.Y.IsNan() ? 0 : source.Y;
        return source;
    }
    
    public static bool IsZeroVector(this Vector2 vector)
    {
        return !vector.HasLength();
    }
    
    public static bool HasLength(this Vector2 vector)
    {
        return vector.X != 0 || vector.Y != 0;
    }
    
    public static Vector2 ToNormalized(this Vector2 vector)
    {
        return Vector2.Normalize(vector);		
    }
		
    public static Vector2 ToNormalizedOrZero(this Vector2 vector)
    {
        if (vector.IsZeroVector())
            return Vector2.Zero;
        else
            return Vector2.Normalize(vector);		
    }
    
    public static bool IsLengthGreaterThan(this Vector2 vector, float length)
    {
        return vector.LengthSquared() > length.Squared();
    }
    
    public static bool IsLengthLessThan(this Vector2 vector, float length)
    {
        return vector.LengthSquared() < length.Squared();
    }
    
    public static Vector2 WithLength(this Vector2 vector, float length)
    {
        return vector.ToNormalized() * length;
    }

    // public static Vector2 ProjectedOnto(this Vector2 sourceVector, Vector2 other) {
    //     // (a*b/b*b)b
    //     return other * (Vector2.Dot(sourceVector, other) / other.LengthSquared());
    // }
    
    public static float DistanceSquaredTo(this Vector2 sourceVector, Vector2 other) {
        return (sourceVector - other).LengthSquared();
    }

    public static bool IsWithin(this Vector2 vector, Rectangle rectangle) {
        return rectangle.Contains(vector);
    }

    public static Vector2 WithMaxLength(this Vector2 source, float length) {
        return source.IsLengthGreaterThan(length) ? source.WithLength(length) : source;
    }
    
    // Min 0
    // public static void SubtractLength(this Vector2 vector, float length) {
    //     if (vector.IsLengthLessThan(length)) {
    //         vector -= vector;
    //     }
    //     else {
    //         vector -= vector.WithLength(length);
    //     }
    // }

    // public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
    // {
    //     return Vector2.Clamp(vector, min, max);
    // }
    //
    // public static Vector2 Min(this Vector2 vector, Vector2 min)
    // {
    //     return Vector2.Min(vector, min);
    // }
    //
    // public static Vector2 Max(this Vector2 vector, Vector2 max)
    // {
    //     vector.X = Math.Min(vector.X,) 
    //     return Vector2.Max(vector, max);
    // }
}