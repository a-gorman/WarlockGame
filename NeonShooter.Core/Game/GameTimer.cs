using System;

namespace NeonShooter.Core.Game;

public class GameTimer
{
    public int FramesRemaining { get; set; }
    
    private const float FramesPerSecond = 60;

    public bool IsExpired => FramesRemaining == 0;

    private GameTimer(int frames)
    {
        FramesRemaining = frames;
    }

    public static GameTimer FromFrames(int frames)
    {
        return new GameTimer(frames);
    }
    
    public static GameTimer FromSeconds(float seconds)
    {
        return new GameTimer((int) (FramesPerSecond * seconds));
    }
    
    public void Update()
    {
        FramesRemaining = Math.Max(0, FramesRemaining - 1);
    }
}