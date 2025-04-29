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

    public GameTimer ToTimer()
    {
        return GameTimer.FromTicks(Ticks);
    }
}