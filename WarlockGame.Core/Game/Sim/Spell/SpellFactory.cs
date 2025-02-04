using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buff;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Sim.Spell.Component;
using WarlockGame.Core.Game.Sim.Spell.Effect;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellFactory {

    private readonly Simulation _simulation;
    
    public SpellFactory(Simulation simulation) {
        _simulation = simulation;
    }

    public WarlockSpell Fireball() {
        return new WarlockSpell(_simulation)
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

    public WarlockSpell Lightning() {
        return new WarlockSpell(_simulation)
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

    public WarlockSpell Poison() {
        return new WarlockSpell(_simulation)
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

    public WarlockSpell Burst() {
        return new WarlockSpell(_simulation)
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

    public WarlockSpell WindShield() {
        return new WarlockSpell(_simulation)
        {
            SpellId = 5,
            CooldownTime = 60,
            SpellIcon = Art.WindWallIcon,
            Effect = new SelfCastPositionComponent
            {
                Component = new EffectComponent {
                    EffectConstructor = (spellContext, location) => new ContinuousSpellEffect
                    {
                        Context = spellContext,
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