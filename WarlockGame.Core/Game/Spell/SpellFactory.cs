using System.Collections.Generic;
using OneOf;
using WarlockGame.Core.Game.Graphics;

namespace WarlockGame.Core.Game.Spell;

static class SpellFactory {
    public static WarlockSpell Fireball() {
        return new WarlockSpell
        {
            SpellId = 1,
            ManaCost = 10,
            CooldownTime = 60,
            SpellIcon = Art.FireballIcon,
            Effects = new List<OneOf<IDirectionalSpellEffect, ILocationSpellEffect, ISelfSpellEffect>>
            {
                new ProjectileEffect(
                    sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .15f),
                    new[]
                    {
                        new Explosion
                        {
                            Force = 100,
                            Damage = 10,
                            Radius = 30,
                            Falloff = 0
                        }
                    }
                )
            }
        };
    }

    public static WarlockSpell Lightning() {
        return new WarlockSpell
        {
            SpellId = 2,
            ManaCost = 10,
            CooldownTime = 60,
            SpellIcon = Art.LightningIcon,
            Effects = new List<OneOf<IDirectionalSpellEffect, ILocationSpellEffect, ISelfSpellEffect>>
            {
                new LightningEffect()
            }
        };
    }
}