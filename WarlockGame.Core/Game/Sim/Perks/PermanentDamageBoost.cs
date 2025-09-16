using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentDamageBoost : PermanentBuffPerk {
    public PermanentDamageBoost(int forceId) : base(type: PerkType.Regeneration, forceId: forceId) { }

    private const float DamageMultiplier = 2;

    protected override Buff CreateBuff() {
        return new DamageBoost(multiplier: DamageMultiplier, duration: null);
    }
}