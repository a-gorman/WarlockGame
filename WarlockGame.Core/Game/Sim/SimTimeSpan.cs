namespace WarlockGame.Core.Game.Sim;

public struct SimTimeSpan
{
    public const float TicksPerSecond = 60f;

    public readonly int Ticks;

    private SimTimeSpan(int ticks)
    {
        Ticks = ticks;
    }

    public static SimTimeSpan OfSeconds(float seconds) => new SimTimeSpan((int)(seconds * TicksPerSecond));
    public static SimTimeSpan OfTicks(int ticks) => new SimTimeSpan(ticks);

    public GameTimer ToTimer()
    {
        return GameTimer.FromTicks(Ticks);
    }
}