using System;
using System.Collections.Generic;

namespace NeonShooter.Core.Game;

public class GameTimer
{
    public int FramesRemaining { get; private set; }

    public bool IsExpired => FramesRemaining == 0;

    private GameTimer(int frames)
    {
        FramesRemaining = frames;
    }

    public static GameTimer FromFrames(int frames)
    {
        return new GameTimer(frames);
    }
    
    public void Update()
    {
        FramesRemaining = Math.Min(0, FramesRemaining - 1);
    }
}