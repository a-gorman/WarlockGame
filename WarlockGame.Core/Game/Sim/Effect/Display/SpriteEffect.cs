using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;

namespace WarlockGame.Core.Game.Sim.Effect.Display;

public class SpriteEffect : IEffect
{
    private readonly Vector2 _position;
    
    private readonly float _orientation;

    private readonly Sprite _sprite;

    private GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public Vector2? Origin { get; init; }
    
    public SpriteEffect(Sprite sprite, Vector2 position, SimTime duration, float orientation = 0)
    {
        _position = position;
        _orientation = orientation;
        _sprite = sprite;
        _timer = duration.ToTimer();
        Origin = null;
    }

    public void Update()
    {
        _timer.Decrement();
    }
    
    public void Draw(Vector2 viewOffset, SpriteBatch spriteBatch)
    {
        if (!_timer.IsExpired)
        {
            _sprite.Draw(spriteBatch, viewOffset + _position, _orientation, origin: Origin);
        }
    }
}