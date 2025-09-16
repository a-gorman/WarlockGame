namespace WarlockGame.Core.Game.Sim;

public struct SimTime
{
    public const float TicksPerSecond = 60f;

    public readonly int Ticks;

    private SimTime(int ticks)
    {
        Ticks = ticks;
    }

    public static SimTime OfSeconds(float seconds) => new SimTime((int)(seconds * TicksPerSecond));
    public static SimTime OfTicks(int ticks) => new SimTime(ticks);
    public static SimTime FromTickDecayRate(float initialValue, float decayPerTick) => new SimTime((int)(initialValue / decayPerTick));
    /// Creates the amount of time needed to decay from 1 at the given rate
    public static SimTime FromTickDecayRate(float decayPerTick) => new SimTime((int)(1f / decayPerTick));

    public GameTimer ToTimer()
    {
        return GameTimer.FromTicks(Ticks);
    }
    
    public float ToTickDecayRate(float startingValue = 1) {
        return startingValue / Ticks;
    }
}