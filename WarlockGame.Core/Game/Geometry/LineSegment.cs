using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Geometry; 

public class LineSegment {
    public Vector2 Start;
    public Vector2 End;

    /// <summary>
    /// The segment's axis aligned bounding box
    /// </summary>
    public Rectangle BoundingBox;
    
    public LineSegment(Vector2 start, Vector2 end) {
        Start = start;
        End = end;

        var left = (int)Math.Min(start.X, end.X);
        var bottom = (int)Math.Min(start.Y, end.Y);

        BoundingBox = new Rectangle(left, bottom, (int)Math.Max(start.X, end.X) - left, (int)Math.Max(start.Y, end.Y) - bottom);
    }

    public Vector2 GetClosetPointTo(Vector2 point) {
        // Convert to local coordinates
        var AC = point - Start;
        var AB = End - Start;

        // Get point D by taking the projection of AC onto AB then adding the offset of A
        var D = AC.ProjectOnto(AB) + Start;

        var AD = D - Start;

        // D might not be on AB so calculate k of D down AB (aka solve AD = k * AB)
        // We can use either component, but choose larger value to reduce the chance of dividing by zero
        var k = AB.X != 0 ? AD.X / AB.X : AD.Y / AB.Y;

        // Check if D is off either end of the line segment
        if (k <= 0.0) {
            return Start;
        } if (k >= 1.0) {
            return End;
        }

        return D;
    }
}