using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentDamageBoostPerk : PermanentBuffPerk {
    public PermanentDamageBoostPerk() : base(type: PerkType.Regeneration) { }

    private const float DamageMultiplier = 2;

    protected override Buff CreateBuff() {
        return new DamageBoost(multiplier: DamageMultiplier, duration: null);
    }
}