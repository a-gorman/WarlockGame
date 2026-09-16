using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Buffs.Components;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Perks;

class ExplodeOnHitPerk : PermanentBuffPerk {
    private const int Damage = 1;

    public ExplodeOnHitPerk() : base(
        9,
        "Explode on hit",
        "Causes you to explode when hit, dealing damage to all enemies nearby",
        Art.BlackHole) { }

    protected override Buff CreateBuff(Simulation sim) {
        return new Buff(Buff.BuffType.ExplodeOnDamage,
            duration: null,
            stacking: Buff.StackingType.None,
            components: [
                new CastWhenDamaged(sim, [
                        new LocationAreaOfEffect {
                            Shape = new CircleTarget(AoeSelectors.AllIgnoreCaster, innerRadius: 50, outerRadius: 100),
                            Components = [
                                new DamageComponent {
                                    Damage = 20,
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