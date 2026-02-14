namespace WarlockGame.Core.Game.Util;

public static class VectorExtensions
{
    extension(Vector2 source) {
        public Vector2 Rotated(float radians) {
            return Vector2.Rotate(source, radians);
        }

        public Vector2 NanToZero() {
            source.X = source.X.IsNan() ? 0 : source.X;
            source.Y = source.Y.IsNan() ? 0 : source.Y;
            return source;
        }

        public bool IsZeroVector()
        {
            return !source.HasLength();
        }

        public bool HasLength()
        {
            return source.X != 0 || source.Y != 0;
        }

        public Vector2 ToNormalized()
        {
            return Vector2.Normalize(source);		
        }

        public Vector2 ToNormalizedOrZero()
        {
            if (source.IsZeroVector())
                return Vector2.Zero;
            else
                return Vector2.Normalize(source);		
        }

        public bool IsLengthGreaterThan(float length)
        {
            return source.LengthSquared() > length.Squared();
        }

        public bool IsLengthLessThan(float length)
        {
            return source.LengthSquared() < length.Squared();
        }

        public Vector2 WithLength(float length)
        {
            return source.ToNormalized() * length;
        }

        public float DistanceSquaredTo(Vector2 other) {
            return (source - other).LengthSquared();
        }

        public Vector2 WithMaxLength(float length) {
            return source.IsLengthGreaterThan(length) ? source.WithLength(length) : source;
        }

        public Vector2 ToNormalVector() {
            return new Vector2(source.Y, -source.X);
        }
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