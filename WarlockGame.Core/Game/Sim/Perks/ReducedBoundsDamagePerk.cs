using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class ReducedBoundsDamagePerk: PermanentBuffPerk {
    private const float Factor = 0.15f;

    protected override Buff CreateBuff() {
        return new DefenseBuff(duration: null) {
            BoundsDefenseModifier = Factor
        };
    }

    public ReducedBoundsDamagePerk() 
        : base(
            id: 5,            
            name: "Reduced Bounds Damage",
            description: "Permanently reduces the damage you take from being out of bounds.",
            texture: Art.ReducedBoundaryDamageIcon) { }
}