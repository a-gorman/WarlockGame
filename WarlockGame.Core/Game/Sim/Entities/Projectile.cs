using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.Sim.Spell.Component;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities;

class Projectile : Entity {
    private static readonly Random _rand = new();
    private readonly IReadOnlyList<ILocationSpellComponent> _effects;
    public SpellContext Context { get; }
    
    public Projectile(Vector2 position, 
        Vector2 velocity, 
        SpellContext context, 
        Sprite sprite, 
        IReadOnlyList<ILocationSpellComponent> effects) : 
        base(sprite, position, radius: 8) {
        Context = context;
        Position = position;
        Velocity = velocity;
        Orientation = Velocity.ToAngle();
        _effects = effects;
        BlocksProjectiles = true;
        PlayerId = context.Caster.PlayerId;
    }

    public override void Update()
    {
        if (Velocity.HasLength())
            Orientation = Velocity.ToAngle();

        Position += Velocity;
        WarlockGame.Grid.ApplyExplosiveForce(0.5f * Velocity.Length(), Position, 80);

        // delete projectiles that go off-screen
        if (!WarlockGame.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;

            for (int i = 0; i < 30; i++)
                WarlockGame.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = _rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
        }
        
        base.Update();
    }

    public override void HandleCollision(Entity other) {
        if (other != Context.Caster && other.BlocksProjectiles) {
            IsExpired = true;
            foreach (var effect in _effects) {
                effect.Invoke(Context, Position);
            }
        }

        base.HandleCollision(other);
    }

    public void Push(float force, Vector2 direction) {
        Velocity += force * direction.ToNormalizedOrZero();
    }
}