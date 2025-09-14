using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim.Order;

namespace WarlockGame.Core.Game.Util;

internal static class Extensions {
    public static Sprite ToSprite(this Texture2D texture) => new Sprite(texture);
    
    public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color,
        float thickness = 2f)
    {
        Vector2 delta = end - start;
        spriteBatch.Draw(Art.Pixel, start, null, color, delta.ToAngle(), new Vector2(0, 0.5f),
            new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
    }

    public static float ToAngle(this Vector2 vector)
    {
        return (float)Math.Atan2(vector.Y, vector.X);
    }

    public static Vector2 ScaleTo(this Vector2 vector, float length)
    {
        return vector * (length / vector.Length());
    }

    public static Point ToPoint(this Vector2 vector)
    {
        return new Point((int)vector.X, (int)vector.Y);
    }

    public static float NextFloat(this Random rand, float minValue, float maxValue)
    {
        return (float)rand.NextDouble() * (maxValue - minValue) + minValue;
    }

    public static Vector2 NextVector2(this Random rand, float minLength, float maxLength)
    {
        double theta = rand.NextDouble() * 2 * Math.PI;
        float length = rand.NextFloat(minLength, maxLength);
        return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
    }

    public static IEnumerable<Rectangle> Subdivide(this Rectangle rectangle, int rectanglesX, int rectanglesY)
    {
        var rectangleWidth = rectangle.Width / rectanglesX;
        var rectangleHeight = rectangle.Height / rectanglesY;
        for (int x = 0; x < rectanglesX; x++)
        {
            for (int y = 0; y < rectanglesY; y++)
            {
                yield return new Rectangle(rectangleWidth * x, rectangleHeight * y, rectangleWidth, rectangleHeight);
            }
        }
    }

    /// <summary>
    /// Coalesces null and empty strings to a default value
    /// </summary>
    /// <param name="source"></param>
    /// <param name="coalescedValue"></param>
    /// <returns></returns>
    public static string NullOrEmptyTo(this string? source, string coalescedValue) {
        return string.IsNullOrEmpty(source) ? coalescedValue : source;
    }

    public static bool IsEmpty(this string source) {
        return source == string.Empty;
    }

    extension<TKey, TValue>(IDictionary<TKey, TValue> source) {
        public void RemoveAll(Func<TKey, TValue, bool> predicate) {
            foreach (var entry in source) {
                if (predicate(entry.Key, entry.Value)) {
                    source.Remove(entry.Key);
                }
            }
        }

        /// <summary>
        /// Adds returns the dictionary value if the key exists.
        /// If the key does not exist, adds the value computed by valueFunc to the dictionary and returns that value.
        /// Note: This method is not thread safe.
        /// </summary>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFunc) {
            if (source.TryGetValue(key, out var existingValue)) {
                return existingValue;
            }

            var newValue = valueFunc.Invoke(key);
            source.Add(key, newValue);
            return newValue;
        }
    }

    public static CastOrder.CastType ToSimType(this CastCommand.CastType source) {
        return source switch {
            CastCommand.CastType.Self => CastOrder.CastType.Self,
            CastCommand.CastType.Location => CastOrder.CastType.Location,
            CastCommand.CastType.Directional => CastOrder.CastType.Directional,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}