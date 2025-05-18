using System;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors;

class DeflectProjectiles : Behavior {

    public Func<Entity, Projectile, Vector2> DeflectionFunc { get; init; } = (_, projectile) => -projectile.Velocity;
    
    public override void OnAdd(Entity entity) {
        entity.OnCollision += OnCollision;
    }

    public override void OnRemove(Entity entity) {
        entity.OnCollision -= OnCollision;
    }

    private void OnCollision(OnCollisionEventArgs args) {
        if (args.Other is Projectile projectile) {
            projectile.Velocity = DeflectionFunc.Invoke(args.Source, projectile);
        }
    }

    // Snell's law: sin(theta1)/sin(theta2) = n2/n1
    // Assumes air has index of refraction = 1
    public static Vector2 OrientedRectangleDiffraction(Entity source, Projectile projectile, float indexOfRefraction) {
        var sides = source.OrientedRectangle.GetSides();
        MonoGame.Extended.Segment2 closestSegment = sides.MinBy(x => x.SquaredDistanceTo(projectile.Position));
        var surfaceNormal = (closestSegment.End - closestSegment.Start).ToNormalVector();

        var angleOfIncidence = surfaceNormal.ToAngle() - projectile.Velocity.ToAngle();
        var angleOfRefraction = float.Asin(float.Sin(angleOfIncidence) / indexOfRefraction);

        if (!angleOfRefraction.IsNan()) {
            return projectile.Velocity.Rotated(angleOfIncidence - angleOfRefraction);
        }

        // Total internal reflection
        return projectile.Velocity.Rotated(2 * angleOfIncidence - float.Pi);
    }
}