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

    private GameTimer? _timer;

    public SpriteEffect(Sprite sprite, Vector2 position, SimTime? duration, float orientation = 0) {
        Position = position;
        Orientation = orientation;
        _sprite = sprite;
        _timer = duration?.ToTimer();
        Origin = null;
    }

    public void Update() {
        _timer = _timer?.Decremented();
        if (_timer?.IsExpired ?? false) {
            IsExpired = true;
        }
    }

    public void Draw(Vector2 viewOffset, SpriteBatch spriteBatch) {
        if (!IsExpired) {
            _sprite.Draw(spriteBatch, viewOffset + Position, new Angle(Orientation), origin: Origin);
        }
    }
}