using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buff;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Sim.Spell.Component;
using WarlockGame.Core.Game.Sim.Spell.Effect;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellFactory {
    private readonly Simulation _simulation;

    public SpellFactory(Simulation simulation) {
        _simulation = simulation;
    }

    public WarlockSpell Fireball() {
        return new WarlockSpell(_simulation) {
            SpellId = 1,
            CooldownTime = 60,
            SpellIcon = Art.FireballIcon,
            Effect = new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .15f),
                [
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget { Radius = 30 },
                        Components = [
                            new DamageComponent { Damage = 10 },
                            new PushComponent { Force = 100 }
                        ]
                    }
                ]
            )
        };
    }

    public WarlockSpell Lightning() {
        return new WarlockSpell(_simulation) {
            SpellId = 2,
            CooldownTime = 60,
            SpellIcon = Art.LightningIcon,
            Effect = new DirectionalAreaOfEffect {
                Shape = new LineTarget { Length = 600, IgnoreCaster = true, Texture = Art.Lightning },
                Effects = new IWarlockComponent[] {
                    new DamageComponent { Damage = 15 },
                    new PushComponent { Force = 100 }
                }
            }
        };
    }

    public WarlockSpell Poison() {
        return new WarlockSpell(_simulation) {
            SpellId = 3,
            CooldownTime = 60,
            SpellIcon = Art.PoisonIcon,
            Effect = new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .15f),
                new[] {
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget { Radius = 20 },
                        Components = [new ApplyBuff(10, caster => new DamageOverTime(caster, 120, 5f / 60))]
                    }
                }
            )
        };
    }

    public WarlockSpell Burst() {
        return new WarlockSpell(_simulation) {
            SpellId = 4,
            CooldownTime = 60,
            SpellIcon = Art.BurstIcon,
            Effect = new SelfAreaOfEffect {
                Shape = new CircleTarget { Radius = 200 },
                Components = new IWarlockComponent[] {
                    new DamageComponent {
                        Damage = 10,
                        SelfFactor = 0.25f
                    },
                    new PushComponent {
                        Force = 150,
                        SelfFactor = 0
                    }
                }
            }
        };
    }

    public WarlockSpell WindShield() {
        return new WarlockSpell(_simulation) {
            SpellId = 5,
            CooldownTime = 60,
            SpellIcon = Art.WindWallIcon,
            Effect = new SelfCastPositionComponent {
                Components = [
                    new EffectComponent {
                        EffectConstructor = (spellContext, location) => new ContinuousSpellEffect {
                            Context = spellContext,
                            Location = location,
                            RepeatEvery = 5,
                            Timer = GameTimer.FromSeconds(4),
                            Components = [
                                new LocationAreaOfEffect {
                                    Shape = new Doughnut {
                                        Radius = 202,
                                        Width = 30,
                                        IgnoreCaster = true,
                                        FalloffFactor = Falloff.None
                                    },
                                    Components = [
                                        new PushComponent {
                                            Force = 1.5f,
                                            SelfFactor = 0,
                                            ProjectileFactor = 1,
                                            DisplacementTransform = (axis1, axis2) => axis1.PerpendicularClockwise()
                                        }
                                    ]
                                }
                            ]
                        }
                    }
                ]
            }
        };
    }

    public WarlockSpell SoulSplit() {
        return new WarlockSpell(_simulation) {
            SpellId = 6,
            CooldownTime = 60,
            SpellIcon = Art.BurstIcon,
            Effect = new ProjectileComponent(Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .15f), 
            [
                new LocationAreaOfEffect {
                    Shape = new CircleTarget { Radius = 20 },
                    Components = [
                        new DamageComponent { Damage = 5 },
                        new TargetLocationComponent {
                            DynamicComponents = [
                                targetInfo =>
                                    new EntityComponent {
                                        EntityConstructor = (spellContext, location) => {
                                            var sim = spellContext.Simulation;

                                            var image = new Entity(new Sprite(Art.Player), spellContext.Simulation) {
                                                Position = location
                                            };
                                            image.AddBehaviors(
                                                new PushShare(targetInfo.Entity.Id, sim),
                                                new DamageShare(targetInfo.Entity.Id, sim),
                                                new TimedLife(SimTimeSpan.OfSeconds(3.5f)),
                                                new Shadow(targetInfo.Entity.Id, sim),
                                                new Yoyo(sim,
                                                    -targetInfo.DisplacementAxis1.WithLength(160).Rotate(float.Pi / 6),
                                                    SimTimeSpan.OfSeconds(0.5f),
                                                    SimTimeSpan.OfSeconds(3)));
                                            return image;
                                        }
                                    },
                                targetInfo =>
                                    new EntityComponent {
                                        EntityConstructor = (spellContext, location) => {
                                            var sim = spellContext.Simulation;

                                            var image = new Entity(new Sprite(Art.Player), spellContext.Simulation) {
                                                Position = location
                                            };
                                            image.AddBehaviors(
                                                new PushShare(targetInfo.Entity.Id, sim),
                                                new DamageShare(targetInfo.Entity.Id, sim),
                                                new TimedLife(SimTimeSpan.OfSeconds(3.5f)),
                                                new Shadow(targetInfo.Entity.Id, sim),
                                                new Yoyo(sim,
                                                    -targetInfo.DisplacementAxis1.WithLength(160).Rotate(-float.Pi / 6),
                                                    SimTimeSpan.OfSeconds(0.5f),
                                                    SimTimeSpan.OfSeconds(3)));
                                            return image;
                                        }
                                    }
                            ]
                        }
                    ]
                }
            ])
        };
    }
}