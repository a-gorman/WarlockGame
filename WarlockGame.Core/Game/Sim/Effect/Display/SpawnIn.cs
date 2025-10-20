using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;

namespace WarlockGame.Core.Game.Sim.Effect.Display;

public class SpawnIn : IEffect
{
    private readonly Vector2 _position;
    
    private readonly float _orientation;

    private const int Duration = 60;
    
    private readonly Sprite _sprite;

    private GameTimer _timer = GameTimer.FromTicks(Duration);

    public bool IsExpired => _timer.IsExpired;

    public SpawnIn(Texture2D image, Vector2 position, float orientation)
    {
        _position = position;
        _orientation = orientation;
        _sprite = new Sprite(image);
    }

    public void Update()
    {
        _timer.Decrement();
    }
    
    public void Draw(Vector2 location, SpriteBatch spriteBatch)
    {
        if (!_timer.IsExpired)
        {
            float factor = 1 - (float)_timer.TicksRemaining / Duration;
            _sprite.Color = Color.White * factor;
            _sprite.Scale = factor;
            _sprite.Draw(spriteBatch, _position, _orientation);
        }
    }
    
    // public override void Draw(SpriteBatch spriteBatch)
    // {
    // 	// if (_timeUntilStart > 0)
    // 	// {
    // 	// 	// Draw an expanding, fading-out version of the sprite as part of the spawn-in effect.
    // 	// 	float factor = _timeUntilStart / 60f;	// decreases from 1 to 0 as the enemy spawns in
    // 	// 	_sprite.Color = Color.White * factor;
    // 	// 	_sprite.Scale = factor;
    // 	// 	_sprite.Draw(spriteBatch, Position, Orientation);
    // 	// }
    //
    // 	base.Draw(spriteBatch);
    // }
}