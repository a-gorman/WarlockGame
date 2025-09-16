using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

class PermanentInvisibilityPerk : PermanentBuffPerk { 
    private const float FadeInDistanceMin = 200;
    private const float FadeInDistanceMax = 300;
    private const float VisibilityDecay = 0.006f;

    public PermanentInvisibilityPerk(int forceId)
        : base(type: PerkType.Invisibility, forceId: forceId) { }

    protected override Buff CreateBuff() {
        return new Invisibility(
            fadeInDistanceMin: FadeInDistanceMin,
            fadeInDistanceMax: FadeInDistanceMax,
            fadeTime: SimTime.FromTickDecayRate(VisibilityDecay),
            duration: null);
    }
}