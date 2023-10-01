using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Entity.Projectile;

namespace NeonShooter.Core.Game.Spell;

class WarlockSpell
{
    public required int ManaCost { get; init; }
    
    public required int CooldownTime { get; init; }
    
    public required Texture2D SpellIcon { get; init; }
    
    public required List<ICastEffect> Effects { get; init; }

    public void Update()
    {
        Cooldown.Update();
    }

    public GameTimer Cooldown { get; private set; } = GameTimer.FromFrames(0);

    public bool OnCooldown => !Cooldown.IsExpired;
    
    public void Cast(Vector2 position, Vector2 direction)
    {
        if (OnCooldown)
        {
            throw new Exception("Cast spell on cooldown");
        }

        Cooldown = GameTimer.FromSeconds(CooldownTime);
        Effects.ForEach(x => x.OnCast(position, direction));
    }
}