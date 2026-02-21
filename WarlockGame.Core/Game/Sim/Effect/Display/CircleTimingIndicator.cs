using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Log;

namespace WarlockGame.Core.Game.Sim.Effect.Display;

class CircleTimingIndicator : IEffect {
    private GameTimer _timer;
    private CircleF _circle;
    private readonly int _initialTicks;
    public bool IsExpired => _timer.IsExpired;

    public CircleTimingIndicator(CircleF shape, SimTime duration) {
        _timer = duration.ToTimer();
        _circle = shape;
        _initialTicks = duration.Ticks;
    }
    
    public void Update() {
        _timer = _timer.Decremented();

        if (_timer.IsExpired) {
            Logger.Debug("Circle expired", Logger.LogType.Simulation);
        }
    }
    
    public void Draw(Vector2 viewOffset, SpriteBatch spriteBatch) {
        var outerCircle = _circle with { Position = _circle.Position + viewOffset };
        var innerCircle = outerCircle with { Radius = outerCircle.Radius * (1 - (float)_timer.TicksRemaining / _initialTicks) };
        spriteBatch.DrawCircle(outerCircle, 30, Color.White);
        spriteBatch.DrawCircle(innerCircle, 30, Color.OrangeRed);
    }
}