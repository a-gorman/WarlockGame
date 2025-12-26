using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class ReducedAllDamagePerk : PermanentBuffPerk {
    private const float Factor = 0.8f;

    protected override Buff CreateBuff() {
        return new DefenseBuff(duration: null) {
            GenericDefenseModifier = Factor
        };
    }

    public ReducedAllDamagePerk()
        : base(
            id: 6,            
            name: "Reduced Damage",
            description: "Permanently reduces the damage you take from all sources.",
            texture: Art.ReducedAllDamageIcon) { }
}