using System;

namespace WarlockGame.Core.Game;

public class GameTimer
{
    public int FramesRemaining { get; set; }
    
    private const float FramesPerSecond = 60;

    public bool IsExpired => FramesRemaining == 0;

    private GameTimer(int frames)
    {
        FramesRemaining = frames;
    }

    public static GameTimer FromTicks(int frames)
    {
        return new GameTimer(frames);
    }
    
    public static GameTimer FromSeconds(float seconds)
    {
        return new GameTimer((int) (FramesPerSecond * seconds));
    }
    
    public bool Update()
    {
        FramesRemaining = Math.Max(0, FramesRemaining - 1);
        return FramesRemaining == 0;
    }
}