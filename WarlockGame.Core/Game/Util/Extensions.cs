using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim.Order;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

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

    extension(CastAction.CastType source) {
        public CastOrder.CastType ToSimType() {
            return source switch {
                CastAction.CastType.Self => CastOrder.CastType.Self,
                CastAction.CastType.Location => CastOrder.CastType.Location,
                CastAction.CastType.Directional => CastOrder.CastType.Directional,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
    }

    extension(Rectangle source) {
        public IEnumerable<Rectangle> Subdivide(int rectanglesX, int rectanglesY) {
            var rectangleWidth = source.Width / rectanglesX;
            var rectangleHeight = source.Height / rectanglesY;

            for (int y = 0; y < rectanglesY; y++) {
                for (int x = 0; x < rectanglesX; x++) {
                    yield return new Rectangle(rectangleWidth * x, rectangleHeight * y, rectangleWidth, rectangleHeight);
                }
            }
        }

        public Rectangle WithMargin(int margin) {
            return new Rectangle(
                source.X + margin,
                source.Y + margin,
                source.Width - 2 * margin,
                source.Height - 2 * margin);
        }
        
        public Rectangle WithMargin(int marginX, int marginY) {
            return new Rectangle(
                source.X + marginX,
                source.Y + marginY,
                source.Width - 2 * marginX,
                source.Height - 2 * marginY);
        }
        
        public Rectangle AtOrigin() {
            return source with { X = 0, Y = 0 };
        }
    }

    extension<T>(T) where T: struct, Enum {
        public static T? ParseOrNull(String s, bool caseInsensitive) {
            if (Enum.TryParse(s, caseInsensitive, out T result)) {
                return result;
            }

            return null;
        }
    }
}