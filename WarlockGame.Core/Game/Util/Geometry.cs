using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace WarlockGame.Core.Game.Util;

public static class Geometry {
    
    /// <summary>
    /// Gets the sides of an oriented rectangle, in clockwise order
    /// </summary>
    public static Segment2[] GetSides(this in OrientedRectangle rectangle) {
        Vector2 position =  new Vector2(rectangle.Radii.X, -rectangle.Radii.Y);
        Vector2 radii = rectangle.Radii;
        var corner1 = Vector2.Transform(radii, rectangle.Orientation) + rectangle.Center;
        var corner2 = Vector2.Transform(position, rectangle.Orientation) + rectangle.Center;
        var corner3 = Vector2.Transform(-radii, rectangle.Orientation) + rectangle.Center;
        var corner4 = Vector2.Transform(-position, rectangle.Orientation) + rectangle.Center;

        return [
            new Segment2(corner1, corner2),
            new Segment2(corner2, corner3),
            new Segment2(corner3, corner4),
            new Segment2(corner4, corner1)
        ];
    }
    
    /// <summary>
    /// Circle-Circle collision
    /// </summary>
    public static bool IsColliding(Vector2 position1, float radius1, Vector2 position2, float radius2) {
        return (position1 - position2).LengthSquared() <= (radius1 + radius2).Squared();
    }
    
    /// <summary>
    /// Circle-Rectangle collision
    /// </summary>
    public static bool IsColliding(Vector2 circlePos, float radius, Rectangle rectangle) {
        // Find the closest point to the circle within the rectangle
        float closestX = Math.Clamp(circlePos.X, rectangle.Left, rectangle.Right);
        float closestY = Math.Clamp(circlePos.Y, rectangle.Top, rectangle.Bottom);

        // Calculate the distance between the circle's center and this closest point
        float distanceX = circlePos.X - closestX;
        float distanceY = circlePos.Y - closestY;

        // If the distance is less than the circle's radius, an intersection occurs
        float distanceSquared = distanceX.Squared() + distanceY.Squared();
        return distanceSquared < radius * radius;
    }
    
    /// <summary>
    /// Circle-Rotated Rectangle collision
    /// </summary>
    public static bool IsColliding(Vector2 circlePos, float radius, BoundingRectangle rectangle, Angle rectRotation) {
        circlePos.RotateAround(rectangle.Center, -rectRotation);
        return new CircleF(circlePos, radius).Intersects(rectangle);
    }
    
    /// <summary>
    /// Circle-Rotated Rectangle collision
    /// </summary>
    public static bool IsColliding(Vector2 circlePos, float radius, RotatedRectangle rectangle) {
        circlePos.RotateAround(rectangle.Rectangle.Center, rectangle.Rotation);   
        return new CircleF(circlePos, radius).Intersects(rectangle.Rectangle);
    }

    /// <summary>
    /// Uses the separating axis theorem to check if two polygons are intersecting
    /// </summary>
    public static bool IsColliding(Polygon poly1, Polygon poly2) {
        Span<Vector2> axes1 = stackalloc Vector2[poly1.Vertices.Length];
        for (int i = 0; i < poly1.Vertices.Length; i++) {
            axes1[i] = poly1.Vertices[i] - poly1.Vertices[(i + 1) % poly1.Vertices.Length].ToNormalVector();
        }
        
        Span<Vector2> axes2 = stackalloc Vector2[poly2.Vertices.Length];
        for (int i = 0; i < poly2.Vertices.Length; i++) {
            axes2[i] = poly2.Vertices[i] - poly2.Vertices[(i + 1) % poly2.Vertices.Length].ToNormalVector();
        }
        
        foreach (var axis in axes1) {
            var projection1 = ProjectShapeOntoVector(axes1, axis);
            var projection2 = ProjectShapeOntoVector(axes2, axis);

            if (projection1.start >= projection2.end || projection1.end <= projection2.start) {
                return false;
            }
        }
        
        foreach (var axis in axes2) {
            var projection1 = ProjectShapeOntoVector(axes1, axis);
            var projection2 = ProjectShapeOntoVector(axes2, axis);

            if (projection1.start >= projection2.end || projection1.end <= projection2.start) {
                return false;
            }
        }

        return true;
    }

    private static (float start, float end) ProjectShapeOntoVector(Span<Vector2> axes, Vector2 vector) {
        float min = 0f;
        float max = 0f;
        foreach (var vertex in axes) {
            var p = vertex.Dot(vector);
            min = Math.Min(min, p);
            max = Math.Max(max, p);
        }

        return (min, max);
    }
    
    public static Polygon CreatePolygonFromRectangle(Rectangle rectangle) {
        var pos = rectangle.Location.ToVector2();
        
        return new Polygon([
            pos,
            pos.Translate(rectangle.Width, 0),
            pos.Translate(0, rectangle.Height),
            pos.Translate(rectangle.Width, rectangle.Height)
        ]);
    }
    
    public static Polygon CreatePolygonFromRectangle(Vector2 center, float width, float height) {
        return new Polygon([
            center.Translate( width / 2,  height / 2),
            center.Translate( width / 2, -height / 2),
            center.Translate(-width / 2,  height / 2),
            center.Translate(-width / 2, -height / 2)
        ]);
    }
    
    public static float GetInteriorAngle(float angle1, float angle2) {
        var difference = (angle2 - angle1) % float.Tau;
        if (Math.Abs(difference) > float.Pi) {
            return difference - float.Tau;
        }
        return difference;
    }
}