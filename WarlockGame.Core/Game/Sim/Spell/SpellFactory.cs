using System.Linq;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buff;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;
using WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors;
using WarlockGame.Core.Game.Sim.Entities.Behaviors.CollisionBehaviors.CollisionFilters;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Sim.Spell.Component;
using WarlockGame.Core.Game.Sim.Spell.Effect;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellFactory {
    private int _nextSpellId = 0;
    private readonly Simulation _simulation;

    public SpellFactory(Simulation simulation) {
        _simulation = simulation;
    }

    public WarlockSpell Fireball() {
        return new WarlockSpell(_simulation) {
            Id = _nextSpellId++,
            SpellTypeId = 1,
            Name = "Fireball",
            CooldownTime = SimTime.OfSeconds(3).Ticks,
            SpellIcon = Art.FireballIcon,
            Effect = new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, 10, scale: .12f),
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
            Id = _nextSpellId++,
            SpellTypeId = 2,
            Name = "Lightning",
            CooldownTime = SimTime.OfSeconds(20).Ticks,
            SpellIcon = Art.LightningIcon,
            Effect = new DirectionalAreaOfEffect {
                Shape = new LineTarget { Length = 600, IgnoreCaster = true, Texture = Art.Lightning },
                Effects = [
                    new DamageComponent { Damage = 15 },
                    new PushComponent { Force = 100 }
                ]
            }
        };
    }

    public WarlockSpell Poison() {
        return new WarlockSpell(_simulation) {
            Id = _nextSpellId++,
            SpellTypeId = 3,
            Name = "Poison",
            CooldownTime = SimTime.OfSeconds(6).Ticks,
            SpellIcon = Art.PoisonIcon,
            Effect = new ProjectileComponent(
                sprite: new Sprite(Art.PoisonBall) { Scale = 0.80f },
                [
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget { Radius = 20 },
                        Components = [new ApplyBuff(10, caster => new DamageOverTime(caster, 120, 5f / 60))]
                    }
                ]
            )
        };
    }

    public WarlockSpell Burst() {
        return new WarlockSpell(_simulation) {
            Id = _nextSpellId++,
            SpellTypeId = 4,
            Name = "Burst",
            CooldownTime = SimTime.OfSeconds(6).Ticks,
            SpellIcon = Art.BurstIcon,
            Effect = new SelfAreaOfEffect {
                Shape = new CircleTarget { Radius = 200 },
                Components = new ITargetComponent[] {
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
            Id = _nextSpellId++,
            SpellTypeId = 5,
            Name = "Wind Shield",
            CooldownTime = SimTime.OfSeconds(16).Ticks,
            SpellIcon = Art.WindWallIcon,
            Effect = new SelfCastPositionComponent {
                Components = [
                    new EffectComponent {
                        EffectConstructor = (spellContext, location) => new ContinuousSpellEffect {
                            Context = spellContext,
                            Location = location,
                            RepeatEvery = 5,
                            Timer = GameTimer.FromSeconds(6),
                            Components = [
                                new LocationAreaOfEffect {
                                    Shape = new Doughnut {
                                        Radius = 202,
                                        Width = 30,
                                        IgnoreCaster = false,
                                        FalloffFactor = Falloff.None
                                    },
                                    Components = [
                                        new PushComponent {
                                            Force = 5f,
                                            SelfFactor = 0,
                                            ProjectileFactor = 1,
                                            DisplacementTransform = (axis1, _) => axis1.PerpendicularClockwise()
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

    public WarlockSpell SoulShatter() {
        return new WarlockSpell(_simulation) {
            Id = _nextSpellId++,
            SpellTypeId = 6,
            Name = "Soul Shatter",
            CooldownTime = SimTime.OfSeconds(6).Ticks,
            SpellIcon = Art.SoulShatterIcon,
            Effect = new ProjectileComponent(Sprite.FromGridSpriteSheet(Art.PowerBall, 4, 4, 10),
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

                                            var image = new Entity(new Sprite(Art.Player), location) {
                                                BlocksProjectiles = true,
                                                PlayerId = spellContext.Caster.PlayerId
                                            };
                                            image.AddBehaviors(
                                                new PushShare(targetInfo.Entity.Id, sim),
                                                new DamageShare(targetInfo.Entity.Id, sim),
                                                new TimedLife(SimTime.OfSeconds(3.5f)),
                                                new Shadow(targetInfo.Entity.Id, sim),
                                                new Yoyo(sim,
                                                    -targetInfo.DisplacementAxis1.WithLength(160).Rotated(float.Pi / 6),
                                                    SimTime.OfSeconds(0.5f),
                                                    SimTime.OfSeconds(3)));
                                            return image;
                                        }
                                    },
                                targetInfo =>
                                    new EntityComponent {
                                        EntityConstructor = (spellContext, location) => {
                                            var sim = spellContext.Simulation;

                                            var image = new Entity(new Sprite(Art.Player), location) {
                                                BlocksProjectiles = true,
                                                PlayerId = spellContext.Caster.PlayerId
                                            };
                                            image.AddBehaviors(
                                                new PushShare(targetInfo.Entity.Id, sim),
                                                new DamageShare(targetInfo.Entity.Id, sim),
                                                new TimedLife(SimTime.OfSeconds(3.5f)),
                                                new Shadow(targetInfo.Entity.Id, sim),
                                                new Yoyo(sim,
                                                    -targetInfo.DisplacementAxis1.WithLength(160)
                                                        .Rotated(-float.Pi / 6),
                                                    SimTime.OfSeconds(0.5f),
                                                    SimTime.OfSeconds(3)));
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

    public WarlockSpell RefractionShield() {
        return new WarlockSpell(_simulation) {
            Id = _nextSpellId++,
            SpellTypeId = 7,
            Name = "Refraction Shield",
            CooldownTime = SimTime.OfSeconds(16).Ticks,
            SpellIcon = Art.RefractionShieldIcon,
            Effect = new SelfCastPositionComponent {
                Components = [
                    new EntityComponent {
                        EntityConstructor = (spellContext, location) => {
                            var caster = spellContext.Caster;
                            var wallLoc = location + new Vector2(80, 40).Rotated(caster.Orientation);

                            return new Entity(new Sprite(Art.Pixel), wallLoc, 5, 50, caster.Orientation + float.Pi / 6) {
                                    PlayerId = caster.PlayerId
                                }
                                .Also(x => x.AddBehaviors(
                                    new DebugVisualize(),
                                    new TimedLife(SimTime.OfSeconds(4)),
                                    new OneCollisionPerEntity(),
                                    new SimpleCollisionFilter(SimpleCollisionFilter.IgnoreFriendlies),
                                    new DeflectProjectiles {
                                        DeflectionFunc = (e, p) =>
                                            DeflectProjectiles.OrientedRectangleDiffraction(e, p, 0.4f)
                                    }
                                ));
                        }
                    },
                    new EntityComponent {
                        EntityConstructor = (spellContext, location) => {
                            var caster = spellContext.Caster;
                            var wallLoc = location + new Vector2(80, -40).Rotated(caster.Orientation);

                            return new Entity(new Sprite(Art.Pixel), wallLoc, 5, 50, caster.Orientation - float.Pi / 6) {
                                    PlayerId = caster.PlayerId
                                }
                                .Also(x => x.AddBehaviors(
                                    new DebugVisualize(),
                                    new TimedLife(SimTime.OfSeconds(4)),
                                    new OneCollisionPerEntity(),
                                    new SimpleCollisionFilter(SimpleCollisionFilter.IgnoreFriendlies),
                                    new DeflectProjectiles {
                                        DeflectionFunc = (e, p) =>
                                            DeflectProjectiles.OrientedRectangleDiffraction(e, p, 0.4f)
                                    }
                                ));
                        }
                    }
                ]
            }
        };
    }

    public WarlockSpell Homing() {
        return new WarlockSpell(_simulation) {
            Id = _nextSpellId++,
            SpellTypeId = 8,
            Name = "Homing",
            CooldownTime = SimTime.OfSeconds(6).Ticks,
            SpellIcon = Art.HomingIcon,
            Effect = new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.EnergySpark, 4, 4, 10, scale: 2f, rotates: false),
                effects: [
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget { Radius = 30 },
                        Components = [
                            new DamageComponent { Damage = 10 },
                            new PushComponent { Force = 100 }
                        ]
                    }
                ],
                behaviors: () => [
                    new AccelerateTowards(0.22f,
                        targetLocation: projectile =>
                            _simulation.EntityManager.Warlocks
                                .Where(x => x.PlayerId != projectile.PlayerId)
                                .MinBy(x => x.Position.DistanceSquaredTo(projectile.Position))
                                ?.Position),
                    new Friction(0.001f, 0.001f, 0.08f),
                    new TimedLife(SimTime.OfSeconds(6))
                ]
            )
        };
    }

    public WarlockSpell Boomerang() {
        return new WarlockSpell(_simulation) {
            Id = _nextSpellId++,
            SpellTypeId = 9,
            Name = "Boomerang",
            CooldownTime = SimTime.OfSeconds(6).Ticks,
            SpellIcon = Art.BoomerangIcon,
            Effect = new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.Triple, 4, 4, 10, scale: 2f),
                speed: 18,
                effects: [
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget { Radius = 30 },
                        Components = [
                            new DamageComponent { Damage = 10 },
                            new PushComponent { Force = 100 }
                        ]
                    }
                ],
                behaviors: () => {
                    var initialTick = _simulation.Tick;
                    return [
                        new AccelerateTowards(0.18f,
                            targetLocation: projectile =>
                                _simulation.EntityManager.Warlocks
                                    .FirstOrDefault(x => x.PlayerId == projectile.PlayerId)
                                    ?.Position),
                        new Friction(c: 0.1f),
                        new TimedLife(SimTime.OfSeconds(9)),
                        new OnCollision(args => {
                            if (args.Source is Projectile projectile
                                && projectile.Context.Caster.Id == args.Other.Id
                                && initialTick + SimTime.OfSeconds(0.5f).Ticks <= _simulation.Tick) {
                                args.Source.IsExpired = true;
                            }
                        })
                    ];
                })
        };
    }
}