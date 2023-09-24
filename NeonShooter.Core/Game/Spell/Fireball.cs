using System;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Spell;

class Fireball : ISpell
{
    public required int ManaCost { get; init; }
    
    public required int CooldownTime { get; init; }

    public void Update()
    {
        Cooldown?.Update();
    }

    public GameTimer Cooldown { get; private set; } = GameTimer.FromFrames(0);

    public bool OnCooldown => !Cooldown.IsExpired;

    private int _speed = 5;

    public void Cast(Vector2 position, Vector2 direction)
    {
        if (OnCooldown)
        {
            throw new Exception("Cast spell on cooldown");
        }

        Cooldown = GameTimer.FromSeconds(CooldownTime);
        EntityManager.Add(new FireballProjectile(position, direction * _speed));
    }
}