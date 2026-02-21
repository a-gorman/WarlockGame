using System;
using System.Linq;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Effect.Display;
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
    private int _nextSpellId = 1;
    private readonly Simulation _simulation;

    public SpellFactory(Simulation simulation) {
        _simulation = simulation;
    }

    public WarlockSpell CreateWarlockSpell(SpellDefinition definition) {
        return new WarlockSpell(_nextSpellId++, definition, _simulation);
    }

    public SpellDefinition Fireball() {
        return new SpellDefinition(
            id: 1,
            name: "Fireball",
            cooldownTime: SimTime.OfSeconds(3),
            spellIcon: Art.FireballIcon,
            effects: new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, SimTime.OfTicks(10), scale: .12f),
                effects: [
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget { Radius = 30 },
                        Components = [
                            new DamageComponent { Damage = 20 },
                            new PushComponent { Force = 150 }
                        ]
                    }
                ]
            )
        );
    }

    public SpellDefinition Lightning() {
        return new SpellDefinition(
            id: 2,
            name: "Lightning",
            cooldownTime: SimTime.OfSeconds(20),
            spellIcon: Art.LightningIcon,
            effects: new DirectionalAreaOfEffect {
                Shape = new LineTarget { Length = 600, IgnoreCaster = true, Texture = Art.Lightning },
                Effects = [
                    new DamageComponent { Damage = 20 },
                    new PushComponent { Force = 100 }
                ]
            }
        );
    }

    public SpellDefinition Poison() {
        return new SpellDefinition(
            id: 3,
            name: "Poison",
            cooldownTime: SimTime.OfSeconds(6),
            spellIcon: Art.PoisonIcon,
            effects: new ProjectileComponent(
                sprite: new Sprite(Art.PoisonBall) { Scale = 0.80f },
                [
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget { Radius = 20 },
                        Components = [
                            new BuffComponent(
                                caster => new DamageOverTime(caster, SimTime.OfSeconds(6), 2f / 60),
                                _ => new DefenseBuff(SimTime.OfSeconds(6)) { GenericDefenseModifier = 1.5f })
                        ]
                    }
                ]
            )
        );
    }

    public SpellDefinition Burst() {
        return new SpellDefinition(
            id: 4,
            name: "Burst",
            cooldownTime: SimTime.OfSeconds(6),
            spellIcon: Art.BurstIcon,
            effects: new SelfAreaOfEffect {
                Shape = new CircleTarget { Radius = 200 },
                Components = [
                    new DamageComponent {
                        Damage = 10,
                        SelfFactor = 0.25f
                    },
                    new PushComponent {
                        Force = 150,
                        SelfFactor = 0
                    }
                ]
            }
        );
    }

    public SpellDefinition WindShield() {
        return new SpellDefinition(
            id: 5,
            name: "Wind Shield",
            cooldownTime: SimTime.OfSeconds(16),
            spellIcon: Art.WindWallIcon,
            effects: new SelfCastPositionComponent {
                Components = [
                    new LocationEffectComponent(
                        (spellContext, location) => new ContinuousSpellEffect {
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
                                            Force = 6f,
                                            SelfFactor = 0,
                                            ProjectileFactor = 1,
                                            DisplacementTransform = (axis1, _) => axis1.PerpendicularClockwise()
                                        }
                                    ]
                                }
                            ]
                        }
                    )
                ]
            }
        );
    }

    public SpellDefinition SoulShatter() {
        return new SpellDefinition(
            id: 6,
            name: "Soul Shatter",
            cooldownTime: SimTime.OfSeconds(6),
            spellIcon: Art.SoulShatterIcon,
            effects: new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.PowerBall, 4, 4, SimTime.OfTicks(10)),
                speed: 16,
                effects: [
                new LocationAreaOfEffect {
                    Shape = new CircleTarget { Radius = 20, IgnoreProjectiles = true },
                    Components = [
                        new DamageComponent { Damage = 5 },
                        new BuffComponent(_ => new Slow(0.66f, SimTime.OfSeconds(3.5f))),
                        new EntityLocationComponent {
                            DynamicComponents = [
                                targetInfo =>
                                    new EntityComponent {
                                        EntityConstructor = (spellContext, location) => {
                                            var sim = spellContext.Simulation;

                                            var image = new Entity(new Sprite(Art.Player), location) {
                                                BlocksProjectiles = true,
                                                ForceId = spellContext.Caster.ForceId
                                            };
                                            image.AddBehaviors(
                                                new PushShare(targetInfo.Entity.Id, sim),
                                                new DamageShare(targetInfo.Entity.Id, sim),
                                                new TimedLife(SimTime.OfSeconds(3.5f)),
                                                new Shadow(targetInfo.Entity.Id, sim),
                                                new Yoyo(sim,
                                                    -targetInfo.OriginTargetDisplacement.WithLength(160).Rotated(float.Pi / 6),
                                                    outwardsTime: SimTime.OfSeconds(0.5f),
                                                    inwardsTime: SimTime.OfSeconds(3)));
                                            return image;
                                        }
                                    },
                                targetInfo =>
                                    new EntityComponent {
                                        EntityConstructor = (spellContext, location) => {
                                            var sim = spellContext.Simulation;

                                            var image = new Entity(new Sprite(Art.Player), location) {
                                                BlocksProjectiles = true,
                                                ForceId = spellContext.Caster.ForceId
                                            };
                                            image.AddBehaviors(
                                                new PushShare(targetInfo.Entity.Id, sim),
                                                new DamageShare(targetInfo.Entity.Id, sim),
                                                new TimedLife(SimTime.OfSeconds(3.5f)),
                                                new Shadow(targetInfo.Entity.Id, sim),
                                                new Yoyo(sim,
                                                    -targetInfo.OriginTargetDisplacement.WithLength(160)
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
        );
    }

    public SpellDefinition DeflectionShield() {
        return new SpellDefinition(
            id: 7,
            name: "Deflection Shield",
            cooldownTime: SimTime.OfSeconds(8),
            spellIcon: Art.RefractionShieldIcon,
            effects: new DirectionalComponent {
                Components = [
                    new EntityComponent {
                        EntityConstructor = (spellContext, direction) => {
                            var caster = spellContext.Caster;
                            var angle = Extensions.ToAngle(direction);
                            var wallLoc = caster.Position + new Vector2(80, 40).Rotated(angle);

                            return new Entity(new Sprite(Art.Pixel), wallLoc, 5, 50, angle + float.Pi / 6) {
                                    ForceId = caster.ForceId
                                }
                                .Also(x => x.AddBehaviors(
                                    new DebugVisualize(),
                                    new TimedLife(SimTime.OfSeconds(4)),
                                    new OneCollisionPerEntity(),
                                    new SimpleCollisionFilter(SimpleCollisionFilter.IgnoreFriendlies),
                                    new DeflectProjectiles {
                                        DeflectionFunc = (e, p) =>
                                            DeflectProjectiles.OrientedRectangleReflection(e, p, 0.4f)
                                    }
                                ));
                        }
                    },
                    new EntityComponent {
                        EntityConstructor = (spellContext, direction) => {
                            var caster = spellContext.Caster;
                            var angle = Extensions.ToAngle(direction);
                            var wallLoc = caster.Position + new Vector2(80, -40).Rotated(angle);

                            return new Entity(new Sprite(Art.Pixel), wallLoc, 5, 50, angle - float.Pi / 6) {
                                    ForceId = caster.ForceId
                                }
                                .Also(x => x.AddBehaviors(
                                    new DebugVisualize(),
                                    new TimedLife(SimTime.OfSeconds(4)),
                                    new OneCollisionPerEntity(),
                                    new SimpleCollisionFilter(SimpleCollisionFilter.IgnoreFriendlies),
                                    new DeflectProjectiles {
                                        DeflectionFunc = (e, p) =>
                                            DeflectProjectiles.OrientedRectangleReflection(e, p, 0.4f)
                                    }
                                ));
                        }
                    }
                ]
            }
        );
    }

    public SpellDefinition Homing() {
        return new SpellDefinition(
            id: 8,
            name: "Homing",
            cooldownTime: SimTime.OfSeconds(8),
            spellIcon: Art.HomingIcon,
            effects: new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.EnergySpark, 4, 4, SimTime.OfTicks(10), scale: 2f, rotates: false),
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
                                .Where(x => x.ForceId != projectile.ForceId)
                                .MinBy(x => x.Position.DistanceSquaredTo(projectile.Position))
                                ?.Position),
                    new Friction(0.001f, 0.001f, 0.08f),
                    new TimedLife(SimTime.OfSeconds(6))
                ]
            )
        );
    }

    public SpellDefinition Boomerang() {
        return new SpellDefinition(
            id: 9,
            name: "Boomerang",
            cooldownTime: SimTime.OfSeconds(6),
            spellIcon: Art.BoomerangIcon,
            effects: new ProjectileComponent(
                sprite: Sprite.FromGridSpriteSheet(Art.Triple, 4, 4, SimTime.OfTicks(2), scale: 2f, rotates: false),
                speed: 18,
                radius: 16,
                effects: [
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget { Radius = 18 },
                        Components = [
                            new DamageComponent { Damage = 15 },
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
                                    .FirstOrDefault(x => x.ForceId == projectile.ForceId)
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
        );
    }

    public SpellDefinition FlameStrike() {
        var radius = 150;
        var delaySeconds = 1.25f;
        var animationDurationSeconds = 0.8f;
        return new SpellDefinition(
            id: 10,
            name: "Light strike",
            cooldownTime: SimTime.OfSeconds(20),
            spellIcon: Art.LightStrikeIcon,
            new LocationEffectComponent(location =>
                new CircleTimingIndicator(new CircleF(location, radius), SimTime.OfSeconds(delaySeconds))),
            new DelayedLocationComponent(
                SimTime.OfSeconds(delaySeconds),
                new LocationAreaOfEffect {
                    Shape = new CircleTarget { Radius = radius },
                    Components = [
                        new DamageComponent { Damage = 40 },
                        new PushComponent { Force = 200 }
                    ]
                }
            ),
            new DelayedLocationComponent(
                SimTime.OfSeconds(delaySeconds - animationDurationSeconds / 4),
                new LocationEffectComponent(location => {
                    var flameStrikeSprite = Sprite.FromGridSpriteSheet(
                        Art.FlameStrike,
                        subdivisionsX: 9,
                        subdivisionsY: 1,
                        timeBetweenTransitions: SimTime.OfSeconds(animationDurationSeconds / 6),
                        scale: 1 / 25f * radius
                    );
                    return new SpriteEffect(flameStrikeSprite, location, SimTime.OfSeconds(animationDurationSeconds))
                    {
                        Origin = new Vector2(flameStrikeSprite.Size.X / 2, flameStrikeSprite.Size.Y / 1.3f)
                    };
                })
            )
        ) { CastRange = 700 };
    }
    
    public SpellDefinition FireSpray() {
        var projectiles = 9;
        var projectileEffects = new IDirectionalSpellComponent[projectiles];
        var spreadAngle = Single.Pi / 14;
        for (int i = 0; i < projectiles; i++) {
            projectileEffects[i] = new DelayedDirectionalComponent(
                SimTime.OfMillis(50*i),
                direction: direction => direction.Rotated(_simulation.Random.NextSingle(-spreadAngle, spreadAngle)),
                components: [new ProjectileComponent(
                    sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, SimTime.OfMillis(100), scale: .12f),
                    speed: 10,
                    behaviors: () => [
                        new SimpleCollisionFilter(SimpleCollisionFilter.IgnoreFriendlies)
                    ],
                    effects: [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget { Radius = 20 },
                            Components = [
                                new DamageComponent { Damage = 6 },
                                new PushComponent { Force = 10 }
                            ]
                        }
                    ]
                ),
            ]);
        }
        
        return new SpellDefinition(
            id: 11,
            name: "Fire Spray",
            cooldownTime: SimTime.OfSeconds(11),
            spellIcon: Art.FireSprayIcon,
            effects: projectileEffects
        );
    }
    
    public SpellDefinition Blink() {
        return new SpellDefinition(
            id: 12,
            name: "Teleport",
            cooldownTime: SimTime.OfSeconds(20),
            spellIcon: Art.TeleportIcon,
            effects: new TeleportComponent()
        ) { CastRange = 600 };
    }
    
    public SpellDefinition Shockwave() {
        return new SpellDefinition(
            id: 13,
            name: "Shockwave",
            cooldownTime: SimTime.OfSeconds(7),
            spellIcon: Art.ShockwaveIcon,
            effects: new DirectionalEffectComponent((spellContext, castLoc, direction) => 
                new ShockwaveEffect(spellContext, 
                    castLoc, 
                    distance: 1500, 
                    velocity: direction.WithLength(6),
                    pushAmount: 4.25f,
                    radius: 80))
        );
    }
}