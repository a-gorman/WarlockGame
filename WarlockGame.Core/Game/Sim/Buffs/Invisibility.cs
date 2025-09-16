using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class Invisibility : Buff {
    public float Visibility {
        get;
        set => field = float.Clamp(value, 0, 1);
    } = 1;

    public float FadeInDistanceMin { get; }
    public float FadeInDistanceMax { get; }
    private float FadeDecay { get; }

    public Invisibility(
        float fadeInDistanceMin,
        float fadeInDistanceMax,
        SimTime fadeTime,
        SimTime? duration) : base(BuffType.Invisibility, duration) {

        FadeInDistanceMin = fadeInDistanceMin;
        FadeInDistanceMax = fadeInDistanceMax;
        FadeDecay = fadeTime.ToTickDecayRate();
    }

    public override void Update(Warlock target) {
        Visibility -= FadeDecay;
        base.Update(target);
    }

    public override void OnAdd(Warlock target) {
        target.OnDamaged += OnDamaged;
        target.SpellCast += OnSpellCast;
    }

    public override void OnRemove(Warlock target) {
        target.OnDamaged -= OnDamaged;
        target.SpellCast -= OnSpellCast;
    }

    public override void OnRespawn() {
        ResetVisibility();
    }

    /// <summary>
    /// Calculates the visibility given the distance from an observer
    /// </summary>
    public float CalculateVisibility(float distance) {
        if (distance >= FadeInDistanceMax)
            return Visibility;
        if (distance > FadeInDistanceMin && distance < FadeInDistanceMax)
            return float.Lerp(Visibility, 1,
                (FadeInDistanceMax - distance) / (FadeInDistanceMax - FadeInDistanceMin));
        // else if we are inside the min, or nan
        return 1;
    }

    private void OnDamaged(OnDamagedEventArgs _) { ResetVisibility(); }
    private void OnSpellCast(Warlock _) { ResetVisibility(); }

    private void ResetVisibility() {
        Visibility = 1;
    }

}