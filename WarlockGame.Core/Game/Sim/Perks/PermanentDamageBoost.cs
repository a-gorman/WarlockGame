using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentDamageBoostPerk : PermanentBuffPerk {
    public PermanentDamageBoostPerk()
        : base(type: PerkType.DamageBoost,
            name: "2x Damage",
            description: "Permanently doubles the damage you deal",
            texture: Art.BigExplosionIcon) { }

    private const float DamageMultiplier = 2;

    protected override Buff CreateBuff() {
        return new DamageBoost(multiplier: DamageMultiplier, duration: null);
    }
}