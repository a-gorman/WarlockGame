using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Display;

namespace NeonShooter.Core.Game.Effect;

public class SpawnIn : IEffect
{
    private readonly Vector2 _position;
    
    private readonly float _orientation;

    private const int Duration = 60;
    
    private readonly Sprite _sprite;

    private readonly GameTimer _timer = GameTimer.FromFrames(Duration);

    public bool IsExpired => _timer.IsExpired;

    public SpawnIn(Texture2D image, Vector2 position, float orientation)
    {
        _position = position;
        _orientation = orientation;
        _sprite = new Sprite(image);
    }

    public void Update()
    {
        _timer.Update();
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_timer.IsExpired)
        {
            float factor = _timer.FramesRemaining / Duration;
            _sprite.Color = Color.White * factor;
            _sprite.Scale = factor;
            _sprite.Draw(spriteBatch, _position, _orientation);
        }
    }
}