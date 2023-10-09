using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter.Core.Game.Graphics.Effect; 

public class VectorEffect : IEffect
{
    private readonly Vector2 _start;
    private readonly Vector2 _end;
    private readonly Color _color;

    private readonly GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public VectorEffect(Vector2 start, Vector2 end, Color color, int duration = 1)
    {
        _start = start;
        _end = end;
        _color = color;
        _timer = GameTimer.FromFrames(duration);
    }

    public void Update()
    {
        _timer.Update();
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_timer.IsExpired)
        {
            var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pointTexture.SetData(new[] { Color.Red });
            
            spriteBatch.DrawLine(_start, _end, _color);
        }
    }
    
}