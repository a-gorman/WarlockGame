using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Spell;

internal class Fireball : Entity
{
    private static readonly Random _rand = new();

    public Fireball(Vector2 position, Vector2 velocity)
    {
        _sprite = new Sprite(Art.Fireball);
        Position = position;
        Velocity = velocity;
        Orientation = Velocity.ToAngle();
        Radius = 8;
    }

    public override void Update()
    {
        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();

        Position += Velocity;
        NeonShooterGame.Grid.ApplyExplosiveForce(0.5f * Velocity.Length(), Position, 80);

        // delete bullets that go off-screen
        if (!NeonShooterGame.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;

            for (int i = 0; i < 30; i++)
                NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                    new ParticleState() { Velocity = _rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });

        }
    }
}