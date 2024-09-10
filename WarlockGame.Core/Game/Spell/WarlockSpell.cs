using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OneOf;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Spell.Component;

namespace WarlockGame.Core.Game.Spell;

class WarlockSpell {
    public required int SpellId { get; init; }
    public required int CooldownTime { get; init; }
    public required Texture2D SpellIcon { get; init; }
    public required OneOf<IDirectionalSpellComponent, ILocationSpellComponent, ISelfSpellComponent> Effect { get; init; }
    public GameTimer Cooldown { get; } = GameTimer.FromTicks(0);
    public bool OnCooldown => !Cooldown.IsExpired;
    
    public void Update() {
        Cooldown.Update();
    }

    public void DoCast(Warlock caster, Vector2 direction) {
        Cooldown.FramesRemaining = CooldownTime;
        Effect.Switch(
            directionalEffect => directionalEffect.Invoke(caster, caster.Position, direction),
            locationEffect => locationEffect.Invoke(caster, direction),
            selfEffect => selfEffect.Invoke(caster)
        );
    }
}