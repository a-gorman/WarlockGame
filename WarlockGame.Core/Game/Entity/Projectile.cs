using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Spell;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Entity;

class Projectile : EntityBase{
    private static readonly Random _rand = new();
    private readonly IReadOnlyList<ILocationSpellEffect> _effects;
    public IEntity Caster { get; }
    
    public Projectile(Vector2 position, Vector2 velocity, IEntity caster, Sprite sprite, IReadOnlyList<ILocationSpellEffect> effects) : 
        base(sprite) {
        Caster = caster;
        Position = position;
        Velocity = velocity;
        Orientation = Velocity.ToAngle();
        Radius = 8;
        _effects = effects;
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

    public void OnCollision()
    {
        IsExpired = true;
        foreach (var effect in _effects) {
            effect.Invoke(Caster, Position);
        }
    }
}