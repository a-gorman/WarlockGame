using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Entity;

namespace NeonShooter.Core.Game.Spell;

class WarlockSpell
{
    public required int SpellId { get; init; }
    
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
    
    public void DoCast(IEntity caster, Vector2 direction)
    {
        Cooldown = GameTimer.FromSeconds(CooldownTime);
        Effects.ForEach(x => x.OnCast(caster, direction));
    }
}