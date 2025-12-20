using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentRegenerationPerk : PermanentBuffPerk {
    public PermanentRegenerationPerk() 
        : base(
            id: 3,
            name: "Permanent Regeneration",
            description: "Permanently grants you health regeneration",
            texture: Art.HealIcon) { }

    private const float RegenAmount = 0.0045f;

    protected override Buff CreateBuff() {
        return new Regeneration(regenAmount: RegenAmount, duration: null);
    }
}