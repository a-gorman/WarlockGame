using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Display;
using WarlockGame.Core.Game.Util;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Spell;

namespace WarlockGame.Core.Game.Entity.Projectile;

internal class FireballProjectile : EntityBase, IProjectile
{
    private static readonly Random _rand = new();
    private const int Force = 100;
    private IReadOnlyList<IProjectileEffect> Effects = new [] { new Explosion { Force = Force, Damage = 10, Radius = 30, Falloff = 0 } };

    public IEntity Parent { get; }
    
    public FireballProjectile(Vector2 position, Vector2 velocity, IEntity parent) : 
        base(Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .15f)) {

        Parent = parent;
        Position = position;
        Velocity = velocity;
        Orientation = Velocity.ToAngle();
        Radius = 8;
    }

    public override void Update()
    {
        if (Velocity.HasLength())
            Orientation = Velocity.ToAngle();

        Position += Velocity;
        WarlockGame.Grid.ApplyExplosiveForce(0.5f * Velocity.Length(), Position, 80);

        // delete bullets that go off-screen
        if (!WarlockGame.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;

            for (int i = 0; i < 30; i++)
                WarlockGame.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = _rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
        }
    }

    public void OnHit()
    {
        IsExpired = true;
        foreach (var effect in Effects) {
            effect.OnImpact(this);
        }
    }
}