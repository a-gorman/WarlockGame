using System;
using System.Linq;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Buffs.Components;
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
    private readonly Simulation _simulation;

    public SpellFactory(Simulation simulation) {
        _simulation = simulation;
    }

    public WarlockSpell CreateWarlockSpell(SpellDefinition definition, int id) {
        return new WarlockSpell(id, definition, _simulation);
    }

    public SpellDefinition Fireball() {
        var damage = 20;
        return new DirectionalSpell(
            id: 1,
            name: "Fireball",
            description: $"Launches a fireball that does {damage}.",
            spellIcon: Art.FireballIcon,
            castSound: Sound.FireBallCast,
            cooldownTime: SimTime.OfSeconds(3),
            effects: [
                new ProjectileComponent(
                    sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, SimTime.OfTicks(40), scale: .12f),
                    effects: [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.All, innerRadius: 8, outerRadius: 50),
                            Sound = Sound.FireHit,
                            Components = [
                                new DamageComponent { Damage = damage },
                                new PushComponent { Force = 150 }
                            ]
                        }
                    ]
                )
            ]);
    }

    public SpellDefinition Lightning() {
        var damage = 30;
        var range = 600;
        return new DirectionalSpell(
            id: 2,
            name: "Lightning",
            description: $"Launches a bolt of lightning that is impossible to dodge or block. Does {damage} damage. {range} range.",
            spellIcon: Art.LightningIcon,
            cooldownTime: SimTime.OfSeconds(20),
            effects: [
                new DirectionalAreaOfEffect {
                    Shape = new LineTarget { Length = range, IgnoreCaster = true, Texture = Art.Lightning },
                    Sound = Sound.Lightning,
                    Effects = [
                        new DamageComponent { Damage = 30 },
                        new PushComponent { Force = 100 }
                    ]
                }
            ]
        );
    }

    public SpellDefinition Poison() {
        var damagePerSecond = 2.5f;
        var duration = SimTime.OfSeconds(10);
        var damageAmp = 1.5f;
        return new DirectionalSpell(
            id: 3,
            name: "Poison",
            description: $"Launches a poisonous ball that does {damagePerSecond} damage per second for {duration.Seconds:0.#} seconds. While poisoned, the target takes {(damageAmp - 1):0.#%} more damage.",
            spellIcon: Art.PoisonIcon,
            cooldownTime: SimTime.OfSeconds(6),
            effects: [
                new ProjectileComponent(
                    sprite: new Sprite(Art.PoisonBall, scale: 0.80f),
                    speed: 9,
                    effects: [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.Warlocks, innerRadius: 8, outerRadius: 20),
                            Components = [
                                new ApplyBuffComponent(
                                    caster => new DamageOverTime(caster, duration, damagePerSecond / 60),
                                    _ => new DefenseBuff(duration) { GenericDefenseModifier = damageAmp })
                            ]
                        }
                    ]
                )
            ]);
    }

    public SpellDefinition Burst() {
        var damage = 15;
        var selfFactor = 0.25f;
        var outerRadius = 100;
        return new SelfCastSpell(
            id: 4,
            name: "Burst",
            description: $"Deals {damage} to all away enemies and destroys projectiles within {100} distance of you. Does {damage * selfFactor:0.##} damage to yourself.",
            spellIcon: Art.BurstIcon,
            cooldownTime: SimTime.OfSeconds(6),
            effects: [
                new LocationAreaOfEffect {
                    Shape = new CircleTarget(AoeSelectors.All, innerRadius: 50, outerRadius: outerRadius),
                    Components = [
                        new DamageComponent {
                            Damage = damage,
                            SelfFactor = selfFactor
                        },
                        new PushComponent {
                            Force = 150,
                            SelfFactor = 0
                        },
                        new DestroyProjectile()
                    ]
                }
            ]
        );
    }

    public SpellDefinition WindShield() {
        var duration = SimTime.OfSeconds(6);
        return new SelfCastSpell(
            id: 5,
            name: "Wind Shield",
            description: $"Creates a circle of fast moving wind that bats away incoming projectiles. Lasts {duration.Seconds:0.#} seconds.",
            spellIcon: Art.WindWallIcon,
            cooldownTime: SimTime.OfSeconds(16),
            effects: [
                new LocationEffectComponent((spellContext, location) =>
                    new ContinuousSpellEffect(
                        context: spellContext,
                        location: location,
                        repeatTime: SimTime.OfTicks(5),
                        duration: duration,
                        components: [
                            new LocationAreaOfEffect {
                                Shape = new Doughnut {
                                    Radius = 200,
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
                    )
                )
            ]
        );
    }

    public SpellDefinition SoulShatter() {
        var slowFactor = 0.66f;
        var damage = 5;
        return new DirectionalSpell(
            id: 6,
            name: "Soul Shatter",
            description: $"Launches a fast moving projectile that shatters the victim's soul into 2 shards and expels them from their body. When a shard takes damage, it shares that damage with the host body. While victim has its soul shattered, it is slowed by {slowFactor:0.##%}. Does {damage} damage.",
            spellIcon: Art.SoulShatterIcon,
            cooldownTime: SimTime.OfSeconds(6),
            effects: [
                new ProjectileComponent(
                    sprite: Sprite.FromGridSpriteSheet(Art.PowerBall, 4, 4, SimTime.OfTicks(160)),
                    speed: 18,
                    effects: [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.Warlocks, innerRadius: 8, outerRadius: 20),
                            Components = [
                                new DamageComponent { Damage = damage },
                                new ApplyBuffComponent(_ => new Slow(slowFactor, SimTime.OfSeconds(3.5f))),
                                new AoeEntityComponent((spellContext, targetInfo) => {
                                        var imageMoveAngle = float.Pi / 6;
                                        return [CreateImage(imageMoveAngle), CreateImage(-imageMoveAngle)];

                                        // Local helper method
                                        Entity CreateImage(float angle) {
                                            var sim = spellContext.Simulation;

                                            var image = new Entity(new Sprite(Art.Ghost, scale: 2f),
                                                targetInfo.TargetPosition, radius: 16) {
                                                BlocksProjectiles = true,
                                                ForceId = spellContext.Caster.ForceId
                                            };
                                            
                                            var maxDisplacement = -targetInfo.OriginTargetDisplacement
                                                .WithLength(160)
                                                .Rotated(angle);
                                            
                                            image.AddBehaviors(
                                                new PushShare(targetInfo.Entity.Id, sim),
                                                new DamageShare(targetInfo.Entity.Id, sim),
                                                new TimedLife(SimTime.OfSeconds(3.5f)),
                                                new Shadow(targetInfo.Entity.Id, sim),
                                                new Yoyo(
                                                    maxDisplacement: maxDisplacement,
                                                    outwardsTime: SimTime.OfSeconds(0.5f),
                                                    inwardsTime: SimTime.OfSeconds(3)));
                                            return image;
                                        }
                                    }
                                )
                            ]
                        }
                    ])
            ]);
    }

    public SpellDefinition DeflectionShield() {
        var duration = SimTime.OfSeconds(4);
        return new DirectionalSpell(
            id: 7,
            name: "Deflection Shield",
            description:
            $"Instantly creates a small shield in front of you that deflects all projectiles. Lasts {duration.Seconds:0.#} seconds.",
            spellIcon: Art.RefractionShieldIcon,
            cooldownTime: SimTime.OfSeconds(8),
            effects: [CreateWall(new Vector2(80, 40), float.Pi / 6), CreateWall(new Vector2(80, -40), -float.Pi / 6)]);

        // Local helper method
        EntityComponent CreateWall(Vector2 locationOffset, float rotation) {
            return new EntityComponent((spellContext, location, direction) => {
                    var caster = spellContext.Caster;
                    var angle = Extensions.ToAngle(direction);
                    var wallLoc = location + locationOffset.Rotated(angle);

                    return new Entity(new Sprite(Art.Pixel), wallLoc, 5, 50, angle + rotation) {
                            ForceId = caster.ForceId
                        }
                        .Also(x => x.AddBehaviors(
                            new DebugVisualize(),
                            new TimedLife(SimTime.OfSeconds(4)),
                            new OneCollisionPerEntity(),
                            new SimpleCollisionFilter(SimpleCollisionFilter.IgnoreFriendlies),
                            new DeflectProjectiles {
                                DeflectionFunc = DeflectProjectiles.OrientedRectangleReflection
                            }
                        ));
                }
            );
        }
    }

    public SpellDefinition Homing() {
        var damage = 10;
        var lifeTime = SimTime.OfSeconds(6);
        return new DirectionalSpell(
            id: 8,
            name: "Homing",
            description: $"Launches a slow moving projectiles that accelerates towards the nearest enemy and deals {damage} damage on impact. Times out after {lifeTime.Seconds:0.#} seconds.",
            spellIcon: Art.HomingIcon,
            cooldownTime: SimTime.OfSeconds(8),
            effects: [
                new ProjectileComponent(
                    sprite: Sprite.FromGridSpriteSheet(Art.EnergySpark, 4, 4, SimTime.OfTicks(160), scale: 2f,
                        rotates: false),
                    effects: [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.All, innerRadius: 8, outerRadius: 30),
                            Components = [
                                new DamageComponent { Damage = damage },
                                new PushComponent { Force = 100 }
                            ]
                        }
                    ],
                    additionalBehaviors: () => [
                        new AccelerateTowards(0.22f,
                            targetLocation: projectile =>
                                _simulation.EntityManager.Warlocks
                                    .Where(x => x.ForceId != projectile.ForceId)
                                    .MinBy(x => x.Position.DistanceSquaredTo(projectile.Position))
                                    ?.Position),
                        new Friction(0.001f, 0.001f, 0.08f),
                        new TimedLife(lifeTime)
                    ]
                )
            ]);
    }

    public SpellDefinition Boomerang() {
        var damage = 15;
        var duration = SimTime.OfSeconds(9);
        return new DirectionalSpell(
            id: 9,
            name: "Boomerang",
            description: $"Launches a large projectile that does {damage} damage that constantly attempts to return to you. Lasts {duration.Seconds:0.#} seconds before timing out.",
            spellIcon: Art.BoomerangIcon,
            cooldownTime: SimTime.OfSeconds(6),
            effects: [
                new ProjectileComponent(
                    sprite: Sprite.FromGridSpriteSheet(Art.Triple, 4, 4, SimTime.OfTicks(32), scale: 2f, rotates: false),
                    speed: 18,
                    radius: 23,
                    effects: [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.All, innerRadius: 23, outerRadius: 27),
                            Components = [
                                new DamageComponent { Damage = damage },
                                new PushComponent { Force = 100 }
                            ]
                        }
                    ],
                    additionalBehaviors: () => {
                        var initialTick = _simulation.Tick;
                        return [
                            new AccelerateTowards(0.18f,
                                targetLocation: projectile =>
                                    _simulation.EntityManager.Warlocks
                                        .FirstOrDefault(x => x.ForceId == projectile.ForceId)
                                        ?.Position),
                            new Friction(c: 0.1f),
                            new TimedLife(duration),
                            new OnCollision(args => {
                                if (args.Source is Projectile projectile
                                    && projectile.Context.Caster.Id == args.Other.Id
                                    && initialTick + SimTime.OfSeconds(0.5f).Ticks <= _simulation.Tick) {
                                    args.Source.IsExpired = true;
                                }
                            })
                        ];
                    })
            ]);
    }

    public SpellDefinition FlameStrike() {
        var radius = 180;
        var delaySeconds = 1.35f;
        var animationDurationSeconds = 0.8f;
        var damage = 80;
        return new LocationSpell(
            id: 10,
            name: "Light strike",
            description: $"After {delaySeconds:0.##} seconds, causes a large area to combust with devastating results. Does up to {damage} damage in a {radius} radius.",
            spellIcon: Art.LightStrikeIcon,
            cooldownTime: SimTime.OfSeconds(20),
            castRange: 700,
            effects: [
                new LocationEffectComponent(location =>
                    new CircleTimingIndicator(new CircleF(location, radius), SimTime.OfSeconds(delaySeconds))),
                new DelayedLocationComponent(
                    SimTime.OfSeconds(delaySeconds),
                    new LocationAreaOfEffect {
                        Shape = new CircleTarget(AoeSelectors.All, radius),
                        Components = [
                            new DamageComponent { Damage = damage },
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
                            loopTime: SimTime.OfSeconds(animationDurationSeconds),
                            scale: 1 / 25f * radius,
                            expiredSpriteBehavior: ExpiredSpriteBehavior.Invisible
                        );
                        return new SpriteEffect(flameStrikeSprite,
                            position: location - new Vector2(0, flameStrikeSprite.Size.Y * 0.35f),
                            duration: SimTime.OfSeconds(animationDurationSeconds));
                    })
                )
            ]);
    }

    public SpellDefinition FireSpray() {
        var projectiles = 9;
        var damage = 6;
        var timeBetweenProjectiles = SimTime.OfMillis(50);
        var projectileEffects = new IDirectionalSpellComponent[projectiles];
        var spreadAngle = Single.Pi / 14;
        for (int i = 0; i < projectiles; i++) {
            projectileEffects[i] = new DelayedDirectionalComponent(
                delay: timeBetweenProjectiles * i,
                direction: direction => direction.Rotated(_simulation.Random.NextSingle(-spreadAngle, spreadAngle)),
                components: [
                    new ProjectileComponent(
                        sprite: Sprite.FromGridSpriteSheet(Art.Fireball, 2, 2, SimTime.OfMillis(400), scale: .12f),
                        speed: 10,
                        additionalBehaviors: () => [
                            new SimpleCollisionFilter(SimpleCollisionFilter.IgnoreFriendlies)
                        ],
                        effects: [
                            new LocationAreaOfEffect {
                                Shape = new CircleTarget(AoeSelectors.All, innerRadius: 8, outerRadius: 20),
                                Sound = Sound.FireHit,
                                Components = [
                                    new DamageComponent { Damage = damage },
                                    new PushComponent { Force = 10 }
                                ]
                            }
                        ]
                    ),
                ]);
        }

        return new DirectionalSpell(
            id: 11,
            name: "Fire Spray",
            description: $"Launches a spray of {projectiles} small projectiles over {(timeBetweenProjectiles * projectiles).Seconds:0.#} seconds that each do {damage} damage.",
            spellIcon: Art.FireSprayIcon,
            castSound: Sound.FireSprayCast,
            cooldownTime: SimTime.OfSeconds(11),
            effects: projectileEffects
        );
    }

    public SpellDefinition Blink() {
        var distance = 350;
        return new LocationSpell(
            id: 12,
            name: "Teleport",
            description: $"You instantly teleport up to {distance} from your current position.",
            spellIcon: Art.TeleportIcon,
            cooldownTime: SimTime.OfSeconds(7),
            castRange: distance,
            effects: [new TeleportComponent()]
        );
    }

    public SpellDefinition Shockwave() {
        return new DirectionalSpell(
            id: 13,
            name: "Shockwave",
            description: "Temp",
            spellIcon: Art.ShockwaveIcon,
            cooldownTime: SimTime.OfSeconds(7),
            effects: [
                new DirectionalEffectComponent((spellContext, castLoc, direction) =>
                    new ShockwaveEffect(spellContext,
                        castLoc,
                        distance: 1500,
                        velocity: direction.WithLength(6),
                        pushAmount: 4.25f,
                        radius: 80))
            ]);
    }

    public SpellDefinition LightningJump() {
        var damage = 10;
        var castRange = 650;
        var jumpTime = 0.75f;
        var travelSprite = Sprite.FromGridSpriteSheet(Art.SparkJumpTravel, 3, 3, SimTime.OfSeconds(0.5f), scale: 2.3f);

        return new LocationSpell(
            id: 14,
            name: "Spark Jump",
            description: $"Jumps to another location up to {castRange} away. Creates an electrical discharges that deals {damage} damage at both the origin and landing points.",
            spellIcon: Art.LightningJumpIcon,
            cooldownTime: SimTime.OfSeconds(20),
            castRange: castRange,
            selfEffects: [
                new LocationAreaOfEffect {
                    Shape = new CircleTarget(AoeSelectors.AllIgnoreCaster, 50),
                    Components = [
                        new DamageComponent { Damage = damage },
                        new PushComponent() { Force = 100 }
                    ]
                },
                new ApplyBuffComponent(context => {
                    var displacement = context.TargetPosition - context.CastFromPosition;
                    return new Buff(
                        type: Buff.BuffType.Jumping,
                        duration: SimTime.OfSeconds(jumpTime),
                        stacking: Buff.StackingType.None,
                        components: [
                            new JumpComponent(context.Simulation, displacement: displacement, height: 10),
                            new SpriteComponent(
                                sprite: travelSprite,
                                spriteOffset: new Vector2(100, 0),
                                sim: context.Simulation),
                            new OnExpiration(context, [
                                new LocationAreaOfEffect {
                                    Shape = new CircleTarget(AoeSelectors.WarlocksIgnoreCaster, 50),
                                    Components = [
                                        new DamageComponent { Damage = damage, SelfFactor = 0 },
                                        new PushComponent() { Force = 100, SelfFactor = 0 }
                                    ]
                                },
                                new LocationEffectComponent(x => new SpriteEffect(
                                    sprite: Sprite.FromGridSpriteSheet(Art.SparkJumpExpand, 5, 2, SimTime.OfSeconds(0.5f), 1.35f),
                                    position: x,
                                    duration: SimTime.OfSeconds(0.5f)))
                            ])
                        ]);
                })
            ],
            effects: [
                new LocationEffectComponent((context, _) => new SpriteEffect(
                    sprite: Sprite.FromGridSpriteSheet(Art.SparkJumpExpand, 5, 2, SimTime.OfSeconds(0.25f), 1.35f),
                    position: context.CastFromPosition,
                    duration: SimTime.OfSeconds(0.5f)))
            ]);
    }

    public SpellDefinition Swap() {
        var maxDistance = 500;

        return new DirectionalSpell(
            id: 15,
            name: "Swap",
            description: $"Launches a projectile that causes you to switch position with anything it touches. After the projectile moves {maxDistance}, you teleport to the projectile, even if nothing is nearby.",
            spellIcon: Art.SwapIcon,
            cooldownTime: SimTime.OfSeconds(12),
            effects: [
                new ProjectileComponent(
                    sprite: Sprite.FromGridSpriteSheet(Art.SwapProjectile, 8, 1, SimTime.OfTicks(30), scale: 1f),
                    maxRange: maxDistance,
                    speed: 11,
                    effects: [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.AllIgnoreCaster, radius: 120),
                            Components = [ new SwapComponent() ]
                        },
                        new LocationEffectComponent(CreateHitEffect)
                    ]
                    // Disabled until order of effects can be assured. Right now effects go swap -> caster effect
                    // casterEffects: [ new LocationEffectComponent(CreateHitEffect) ]
                )
        ]);

        static SpriteEffect CreateHitEffect(Vector2 x) =>
            new SpriteEffect(Sprite.FromGridSpriteSheet(Art.SwapEffect, 9, 1,
                    SimTime.OfSeconds(0.5f), scale: 3.3f, expiredSpriteBehavior: ExpiredSpriteBehavior.Invisible),
                position: x,
                duration: SimTime.OfSeconds(0.5f));
    }

    public SpellDefinition GraveTick() {
        var damage = 6;
        var timeBetweenTicks = SimTime.OfSeconds(2);
        var duration = SimTime.OfSeconds(10);
        var shareRadius = 120;
        
        return new DirectionalSpell(
            id: 16,
            name: "Grave Tick",
            description: $"Curses the victim to pulse with death energy. Every {timeBetweenTicks.Seconds:0.#} seconds, the target takes {damage} damage. The damage is divided between all targets within {shareRadius} of the target. Lasts {duration.Seconds:0.#} seconds.",
            spellIcon: Art.GraveTickIcon,
            cooldownTime: SimTime.OfSeconds(16),
            effects: [
                new ProjectileComponent(
                    sprite: Sprite.FromGridSpriteSheet(Art.BraveTickProjectile, 10, 1, SimTime.OfTicks(40), scale: 1f),
                    effects: [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.Warlocks, radius: 40),
                            Components = [
                                new ApplyBuffComponent(context => {
                                    return new Buff(
                                        type: Buff.BuffType.GraveTick,
                                        duration: duration,
                                        stacking: Buff.StackingType.Stacks,
                                        components: [
                                            new OnUpdate(
                                                context: context,
                                                repeatEvery: timeBetweenTicks,
                                                components: [
                                                    new LocationEffectComponent(x => 
                                                        new SpriteEffect(
                                                            sprite: Sprite.FromGridSpriteSheet(Art.GraveTickExplosion, 4, 4, SimTime.OfSeconds(0.5f), 4.5f, opacity: 0.55f),
                                                            position: x,
                                                            onUpdate: static (sprite, transformId, timeElapsed) => { sprite.ChangeTransformation(transformId, 1.8f/30f * timeElapsed.Ticks); },
                                                            duration: SimTime.OfSeconds(0.5f))),
                                                    new LocationAreaOfEffect {
                                                        Shape = new CircleTarget(AoeSelectors.Warlocks, shareRadius),
                                                        Components = [
                                                            new DamageComponent { Damage = damage, SelfFactor = 0, Distributed = true }
                                                        ]
                                                    }
                                                ])
                                        ]);
                                })
                            ]
                        }
                    ]
                )
            ]);
    }

    public SpellDefinition Redirection() {
        return new DirectionalSpell(
            id: 17,
            name: "Redirection",
            description: "Temp",
            spellIcon: Art.LightningJumpIcon,
            cooldownTime: SimTime.OfSeconds(1),
            effects: [
                new DirectionalEffectComponent((spellContext, castLoc, direction) =>
                    new ContinuousSpellEffect(
                        context: spellContext,
                        location: castLoc,
                        repeatTime: SimTime.OfTicks(5),
                        duration: SimTime.OfSeconds(0.5f),
                        components: [
                            new LocationAreaOfEffect {
                                Shape = new CircleTarget(
                                    radius: 50,
                                    targetSelector: AoeSelectors.Projectiles),
                                Components = [
                                    new RedirectProjectileComponent(direction)
                                ]
                            }
                        ]
                    )
                )
            ]);
    }
}