using WarlockGame.Core.Game.Buff;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Spell.Effect;

namespace WarlockGame.Core.Game.Spell;

static class SpellFactory {
    public static WarlockSpell Fireball() {
        return new WarlockSpell
        {
            SpellId = 1,
            CooldownTime = 60,
            SpellIcon = Art.FireballIcon,
            Effect = new ProjectileEffect(
                sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .15f),
                new[]
                {
                    new LocationAreaOfEffect
                    {
                        Shape = new CircleTarget { Radius = 30 },
                        Effects = new IWarlockEffect[]
                        {
                            new DamageEffect { Damage = 10 },
                            new PushEffect { Force = 100 }
                        }
                    }
                }
            )
        };
    }

    public static WarlockSpell Lightning() {
        return new WarlockSpell
        {
            SpellId = 2,
            CooldownTime = 60,
            SpellIcon = Art.LightningIcon,
            Effect = new DirectionalAreaOfEffect
            {
                Shape = new LineTarget { Length = 600 },
                Effects = new IWarlockEffect[]
                {
                    new DamageEffect
                    {
                        Damage = 15
                    },
                    new PushEffect
                    {
                        Force = 10
                    }
                }
            }
        };
    }

    public static WarlockSpell Poison() {
        return new WarlockSpell
        {
            SpellId = 3,
            CooldownTime = 60,
            SpellIcon = Art.LightningIcon,
            Effect = new ProjectileEffect(
                sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .15f),
                new[]
                {
                    new LocationAreaOfEffect
                    {
                        Shape = new CircleTarget { Radius = 20 },
                        Effects = new[] { new ApplyBuff(10, caster => new DamageOverTime(caster, 120, 5f / 60)) }
                    }
                }
            )
        };
    }

    public static WarlockSpell Burst() {
        return new WarlockSpell
        {
            SpellId = 4,
            CooldownTime = 60,
            SpellIcon = Art.FireballIcon,
            Effect = new SelfAreaOfEffect
            {
                Shape = new CircleTarget { Radius = 200 },
                Effects = new IWarlockEffect[]
                {
                    new DamageEffect
                    {
                        Damage = 10,
                        SelfFactor = 0.25f
                    },
                    new PushEffect
                    {
                        Force = 10,
                        SelfFactor = 0
                    }
                }
            }
        };
    }
}