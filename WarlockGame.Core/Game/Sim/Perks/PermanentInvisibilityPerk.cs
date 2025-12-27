using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentInvisibilityPerk : PermanentBuffPerk { 
    private const float FadeInDistanceMin = 1000;
    private const float FadeInDistanceMax = 1300;
    private readonly SimTime _visibilityFadeTime = SimTime.OfSeconds(2.5f);

    public PermanentInvisibilityPerk()
        : base(
            id: 2,
            name: "Permanent Invisibility",
            description: "Permanently grants you invisibility from distant enemies",
            texture: Art.InvisibilityIcon) { }

    protected override Buff CreateBuff() {
        return new Invisibility(
            fadeInDistanceMin: FadeInDistanceMin,
            fadeInDistanceMax: FadeInDistanceMax,
            fadeTime: _visibilityFadeTime,
            duration: null);
    }
}