using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentInvisibilityPerk : PermanentBuffPerk { 
    private const float FadeInDistanceMin = 200;
    private const float FadeInDistanceMax = 350;
    private const int fadeInTimeSeconds = 3;
    private readonly SimTime _visibilityFadeTime = SimTime.OfSeconds(fadeInTimeSeconds);

    public PermanentInvisibilityPerk()
        : base(
            id: 2,
            name: "Permanent Invisibility",
            description: $"Permanently grants you invisibility from distant enemies. Takes {fadeInTimeSeconds:0.#} seconds to fully become invisible again after casting a spell or taking damage.",
            texture: Art.InvisibilityIcon) { }

    protected override Buff CreateBuff(Simulation _) {
        return new Invisibility(
            fadeInDistanceMin: FadeInDistanceMin,
            fadeInDistanceMax: FadeInDistanceMax,
            fadeTime: _visibilityFadeTime,
            duration: null);
    }
}