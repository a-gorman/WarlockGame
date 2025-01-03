using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buff;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Sim.Spell.Component;
using WarlockGame.Core.Game.Sim.Spell.Effect;

namespace WarlockGame.Core.Game.Sim.Spell;

static class SpellFactory {
    public static WarlockSpell Fireball() {
        return new WarlockSpell
        {
            SpellId = 1,
            CooldownTime = 60,
            SpellIcon = Art.FireballIcon,
            Effect = new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .15f),
                new[]
                {
                    new LocationAreaOfEffect
                    {
                        Shape = new CircleTarget { Radius = 30 },
                        Effects = new IWarlockComponent[]
                        {
                            new DamageComponent { Damage = 10 },
                            new PushComponent { Force = 100 }
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
                Shape = new LineTarget { Length = 600, IgnoreCaster = true, Texture = Art.Lightning },
                Effects = new IWarlockComponent[]
                {
                    new DamageComponent { Damage = 15 },
                    new PushComponent { Force = 100 }
                }
            }
        };
    }

    public static WarlockSpell Poison() {
        return new WarlockSpell
        {
            SpellId = 3,
            CooldownTime = 60,
            SpellIcon = Art.PoisonIcon,
            Effect = new ProjectileComponent(
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
            SpellIcon = Art.BurstIcon,
            Effect = new SelfAreaOfEffect
            {
                Shape = new CircleTarget { Radius = 200 },
                Components = new IWarlockComponent[]
                {
                    new DamageComponent
                    {
                        Damage = 10,
                        SelfFactor = 0.25f
                    },
                    new PushComponent
                    {
                        Force = 150,
                        SelfFactor = 0
                    }
                }
            }
        };
    }

    public static WarlockSpell WindShield() {
        return new WarlockSpell
        {
            SpellId = 5,
            CooldownTime = 60,
            SpellIcon = Art.WindWallIcon,
            Effect = new SelfCastPositionComponent
            {
                Component = new EffectComponent {
                    EffectConstructor = (caster, location) => new ContinuousSpellEffect
                    {
                        Caster = caster,
                        Location = location,
                        RepeatEvery = 5,
                        Timer = GameTimer.FromSeconds(4),
                        Components = new[]
                        {
                            new LocationAreaOfEffect
                            {
                                Shape = new Doughnut
                                {
                                    Radius = 202,
                                    Width = 30,
                                    IgnoreCaster = true,
                                    FalloffFactor = Falloff.None
                                },
                                Effects = new[]
                                {
                                    new PushComponent
                                    {
                                        Force = 1.5f,
                                        SelfFactor = 0,
                                        ProjectileFactor = 1,
                                        DisplacementTransform = (axis1, axis2) => axis1.PerpendicularClockwise()
                                    }
                                }
                            }
                        },
                    }
                }
            }
        };
    }
}