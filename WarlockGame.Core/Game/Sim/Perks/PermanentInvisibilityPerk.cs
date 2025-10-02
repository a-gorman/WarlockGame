using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentInvisibilityPerk : PermanentBuffPerk { 
    private const float FadeInDistanceMin = 200;
    private const float FadeInDistanceMax = 400;
    private const float VisibilityDecay = 0.004f;

    public PermanentInvisibilityPerk()
        : base(type: PerkType.Invisibility,
            name: "Permanent Invisibility",
            description: "Permanently grants you invisibility from distant enemies",
            texture: Art.InvisibilityIcon) { }

    protected override Buff CreateBuff() {
        return new Invisibility(
            fadeInDistanceMin: FadeInDistanceMin,
            fadeInDistanceMax: FadeInDistanceMax,
            fadeTime: SimTime.FromTickDecayRate(VisibilityDecay),
            duration: null);
    }
}