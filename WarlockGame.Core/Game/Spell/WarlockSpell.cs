using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OneOf;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell;

class WarlockSpell {
    public required int SpellId { get; init; }
    public required int ManaCost { get; init; }
    public required int CooldownTime { get; init; }
    public required Texture2D SpellIcon { get; init; }
    public required List<OneOf<IDirectionalSpellEffect, ILocationSpellEffect, ISelfSpellEffect>> Effects { get; init; }

    public void Update() {
        Cooldown.Update();
    }

    public GameTimer Cooldown { get; private set; } = GameTimer.FromFrames(0);

    public bool OnCooldown => !Cooldown.IsExpired;

    public void DoCast(IEntity caster, Vector2 direction) {
        Cooldown.FramesRemaining = CooldownTime;
        foreach (var effect in Effects)
            effect.Switch(
                x => x.Invoke(caster, caster.Position, direction),
                x => x.Invoke(caster, direction),
                x => x.Invoke(caster)
            );
    }
}