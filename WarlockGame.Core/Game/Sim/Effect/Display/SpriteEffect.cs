using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;

namespace WarlockGame.Core.Game.Sim.Effect.Display;

public class SpriteEffect : IEffect
{
    public Vector2 Position { get; set; }
    
    public float Orientation { get; set; }

    public bool IsExpired { get; set; }

    public Vector2? Origin { get; init; }
    
    private readonly Sprite _sprite;
    private readonly Action<Sprite, int, SimTime>? _onUpdate;
    private int _transformationId;
    
    private GameTimer? _timer;
    private int _ticksElapsed;

    public SpriteEffect(
        Func<Sprite> spriteCtor,
        Vector2 position,
        SimTime? duration,
        Action<Sprite, int, SimTime>? onUpdate = null,
        float orientation = 0) {
        
        Position = position;
        Orientation = orientation;
        _sprite = spriteCtor.Invoke();
        _onUpdate = onUpdate;
        _timer = duration?.ToTimer();
        Origin = null;

        if (_onUpdate != null) {
            _transformationId = _sprite.AddTransformation();
        }
    }

    public void Update() {
        _ticksElapsed++;
        _timer = _timer?.Decremented();
        if (_timer?.IsExpired ?? false) {
            IsExpired = true;
        }

        if (_onUpdate != null) {
            _onUpdate?.Invoke(_sprite, _transformationId, SimTime.OfTicks(_ticksElapsed));
        }
    }

    public void Draw(Vector2 viewOffset, SpriteBatch spriteBatch) {
        if (!IsExpired) {
            _sprite.Draw(spriteBatch, viewOffset + Position, new Angle(Orientation), origin: Origin);
        }
    }
}