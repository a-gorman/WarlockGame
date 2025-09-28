using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentRegenerationPerk : PermanentBuffPerk {
    public PermanentRegenerationPerk() : base(type: PerkType.DamageBoost) { }

    private const float RegenAmount = 0.0045f;

    protected override Buff CreateBuff() {
        return new Regeneration(regenAmount: RegenAmount, duration: null);
    }
}