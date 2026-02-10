using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class ReducedAllDamagePerk : PermanentBuffPerk {
    private const float Factor = 0.60f;

    protected override Buff CreateBuff() {
        return new DefenseBuff(duration: null) {
            GenericDefenseModifier = Factor
        };
    }

    public ReducedAllDamagePerk()
        : base(
            id: 7,            
            name: "Reduced Damage",
            description: $"Permanently reduces the damage you take from all sources by {(1 - Factor) * 100}%.",
            texture: Art.ReducedAllDamageIcon) { }
}