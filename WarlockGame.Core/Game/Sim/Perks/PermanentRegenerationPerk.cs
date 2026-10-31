using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentRegenerationPerk : PermanentBuffPerk {
    public PermanentRegenerationPerk() 
        : base(
            id: 3,
            name: "Permanent Regeneration",
            description: $"Permanently causes you to regenerate {RegenAmountPerSecond:0.##}hp per second",
            texture: Art.HealIcon) { }

    private const float RegenAmountPerSecond = 1f;

    protected override Buff CreateBuff(Simulation _) {
        return new Regeneration(regenAmount: RegenAmountPerSecond/60, duration: null);
    }
}