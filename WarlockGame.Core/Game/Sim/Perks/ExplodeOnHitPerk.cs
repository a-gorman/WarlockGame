using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Buffs.Components;
using WarlockGame.Core.Game.Sim.Effect.Display;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Perks;

class ExplodeOnHitPerk : PermanentBuffPerk {
    private const int Damage = 20;
    private const int Radius = 100;
    private const int slowTimeSeconds = 2;
    
    public ExplodeOnHitPerk() : base(
        9,
        "Explode on hit",
        $"Causes you to explode when hit, dealing {Damage} damage and slowing all enemies by 30% within {Radius} distance. Limit once per second.",
        Art.ExplodeOnDamageIcon) { }

    protected override Buff CreateBuff(Simulation sim) {
        return new Buff(Buff.BuffType.ExplodeOnDamage,
            duration: null,
            stacking: Buff.StackingType.None,
            components: [
                new CastWhenDamaged(sim, SimTime.OfSeconds(1), [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.AllIgnoreCaster, innerRadius: 50, outerRadius: Radius),
                            Components = [
                                new DamageComponent {
                                    Damage = Damage,
                                    SelfFactor = 0.0f
                                },
                                new PushComponent {
                                    Force = 150,
                                    SelfFactor = 0
                                },
                                new ApplyBuffComponent(_ => new Slow(0.70f, SimTime.OfSeconds(slowTimeSeconds))),
                                new DestroyProjectile()
                            ]
                        },
                        new LocationEffectComponent(x => 
                            new SpriteEffect(
                                sprite: Sprite.FromGridSpriteSheet(Art.ExplodeOnDamageEffect, 8, 1, SimTime.OfSeconds(0.75f), 2f, opacity: 0.8f, expiredSpriteBehavior: ExpiredSpriteBehavior.Invisible),
                                position: x,
                                duration: SimTime.OfSeconds(0.75f)))
                    ]
                )
            ]
        );
    }
}