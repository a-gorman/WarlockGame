using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentRegenerationPerk : PermanentBuffPerk {
    private const float RegenAmountPerSecond = 3.5f;
    private const int DamageDisableTime = 5;
    
    public PermanentRegenerationPerk() 
        : base(
            id: 3,
            name: "Permanent Regeneration",
            description: $"Permanently causes you to regenerate {RegenAmountPerSecond:0.##}hp per second. When you take damage regeneration is disabled for {DamageDisableTime:0.##} seconds.",
            texture: Art.HealIcon) { }

    protected override Buff CreateBuff(Simulation _) {
        return new Regeneration(regenAmount: RegenAmountPerSecond/60, duration: null, damageDisableTime: SimTime.OfSeconds(DamageDisableTime));
    }
}