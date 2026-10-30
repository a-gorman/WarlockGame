using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Buffs.Components;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Perks;

class ExplodeOnHitPerk : PermanentBuffPerk {
    private const int Damage = 20;

    public ExplodeOnHitPerk() : base(
        9,
        "Explode on hit",
        $"Causes you to explode when hit, {Damage} dealing damage to all enemies nearby. Limit once per second.",
        Art.ExplodeOnDamageIcon) { }

    protected override Buff CreateBuff(Simulation sim) {
        return new Buff(Buff.BuffType.ExplodeOnDamage,
            duration: null,
            stacking: Buff.StackingType.None,
            components: [
                new CastWhenDamaged(sim, SimTime.OfSeconds(1), [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.AllIgnoreCaster, innerRadius: 50, outerRadius: 100),
                            Components = [
                                new DamageComponent {
                                    Damage = Damage,
                                    SelfFactor = 0.0f
                                },
                                new PushComponent {
                                    Force = 150,
                                    SelfFactor = 0
                                },
                                new DestroyProjectile()
                            ]
                        }
                    ]
                )
            ]
        );
    }
}