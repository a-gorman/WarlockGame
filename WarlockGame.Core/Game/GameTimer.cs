using System;

namespace WarlockGame.Core.Game;

public readonly struct GameTimer
{
    public int TicksRemaining { get; }
    
    private const float TicksPerSecond = 60;

    public bool IsExpired => TicksRemaining == 0;

    private GameTimer(int ticks)
    {
        TicksRemaining = ticks;
    }

    public static GameTimer FromTicks(int frames)
    {
        return new GameTimer(frames);
    }
    
    public static GameTimer FromSeconds(float seconds)
    {
        return new GameTimer((int) (TicksPerSecond * seconds));
    }
    
    public GameTimer Decremented()
    {
        return new GameTimer(Math.Max(0, TicksRemaining - 1));
    }
}