using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buff;

class Invisibility: IBuff {
   
    public int Id { get; set; }
    public IBuff.BuffType Type => IBuff.BuffType.Invisibility;
    public bool IsExpired { get; set; }
    
    public float Visibility { 
        get; 
        set => field = float.Clamp(value, 0, 1);
    } = 1;

    public float FadeInDistanceMin { get; set; }
    public float FadeInDistanceMax { get; set; }
    private float VisibilityDecay { get; set; }
    
    private GameTimer? Timer { get; set; }

    public Invisibility(
        float fadeInDistanceMin, 
        float fadeInDistanceMax, 
        float visibilityDecay, 
        SimTime? timer = null) {
        
        FadeInDistanceMin = fadeInDistanceMin;
        FadeInDistanceMax = fadeInDistanceMax;
        VisibilityDecay = visibilityDecay;
        Timer = timer?.ToTimer();
    }
    
    public void Update(Warlock target) {
        Timer = Timer?.Decrement() ?? null;
        IsExpired = Timer?.IsExpired ?? false;
    }
    
    public float CalculateVisibility(float distance) {
        if (distance >= FadeInDistanceMax)
            return Visibility;
        if (distance > FadeInDistanceMin && distance < FadeInDistanceMax)
            return float.Lerp(Visibility, 1,
                (FadeInDistanceMax - distance) / (FadeInDistanceMax - FadeInDistanceMin));
        // else if we are inside the min, or nan
        return 1;
    }
}