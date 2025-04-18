using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;

namespace WarlockGame.Core.Game.Sim.Effect.Display;

public class Lightning : IEffect
{
    private readonly Vector2 _position;
    
    private readonly float _orientation;

    private const int Duration = 10;
    
    private readonly Sprite _sprite;

    private readonly GameTimer _timer = GameTimer.FromTicks(Duration);

    public bool IsExpired => _timer.IsExpired;

    public Lightning(Texture2D image, Vector2 position, float orientation)
    {
        _position = position;
        _orientation = orientation;
        _sprite = new Sprite(image);
    }

    public void Update()
    {
        _timer.Decrement();
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_timer.IsExpired)
        {
            _sprite.Color = Color.White;
            _sprite.Scale = 1;
            _sprite.Draw(spriteBatch, _position, _orientation, new Vector2(0, _sprite.Size.Y / 2));
        }
    }
}