using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

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
        _timer = GameTimer.FromFrames(duration+1);
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
            pointTexture.SetData(new[] { Color.White });
            
            spriteBatch.DrawLine(_start, _end, _color);
        }
    }

    public class PointEffect : IEffect {
        private readonly Vector2 _position;
        private readonly Color _color;

        private readonly GameTimer _timer;

        public bool IsExpired => _timer.IsExpired;

        public PointEffect(Vector2 position, Color color, int duration = 1) {
            _position = position;
            _color = color;
            _timer = GameTimer.FromFrames(duration+1);
        }

        public void Update() {
            _timer.Update();
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (!_timer.IsExpired) {
                var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 3, 3);
                pointTexture.SetData(new[] { Color.White });

                spriteBatch.Draw(pointTexture, _position, null, _color);
            }
        }
    }
}

public class StringEffect : IEffect {
    private readonly string _displayString;
    private readonly Vector2 _position;
    private readonly Color _color;

    private readonly GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public StringEffect(string displayString, Vector2 position, Color color, int duration = 1) {
        _displayString = displayString;
        _position = position;
        _color = color;
        _timer = GameTimer.FromFrames(duration+1);
    }

    public void Update() {
        _timer.Update();
    }

    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.DrawString(Art.Font, _displayString, _position, _color);
    }
}

public class CircleEffect : IEffect {
    private readonly CircleF _circle;
    private readonly Vector2 _position;
    private readonly Color _color;

    private readonly GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public CircleEffect(float radius, Vector2 position, Color color, int duration = 1) {
        _circle = new CircleF(position.ToPoint(), radius);
        _position = position;
        _color = color;
        _timer = GameTimer.FromFrames(duration+1);
    }

    public void Update() {
        _timer.Update();
    }

    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.DrawCircle(_circle, 30, _color);
    }
}