using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Effect.Display;

public class VectorEffect : IEffect {
    private readonly Vector2 _start;
    private readonly Vector2 _end;
    private readonly Color _color;

    private GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public VectorEffect(Vector2 start, Vector2 end, Color color, int duration) {
        _start = start;
        _end = end;
        _color = color;
        _timer = GameTimer.FromTicks(duration + 1);
    }

    public void Update() {
        _timer.Decrement();
    }

    public void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (!_timer.IsExpired) {
            var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pointTexture.SetData(new[] { Color.White });

            Extensions.DrawLine(spriteBatch, _start + location, _end + location, _color);
        }
    }
}

public class PointEffect : IEffect {
    private readonly Vector2 _position;
    private readonly Color _color;

    private GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public PointEffect(Vector2 position, Color color, int duration) {
        _position = position;
        _color = color;
        _timer = GameTimer.FromTicks(duration + 1);
    }

    public void Update() {
        _timer = _timer.Decrement();
    }

    public void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (!_timer.IsExpired) {
            var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 3, 3);
            pointTexture.SetData(new[] { Color.White });

            spriteBatch.Draw(pointTexture, _position + location, null, _color);
        }
    }
}

public class StringEffect : IEffect {
    private readonly string _displayString;
    private readonly Vector2 _position;
    private readonly Color _color;

    private GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public StringEffect(string displayString, Vector2 position, Color color, int duration) {
        _displayString = displayString;
        _position = position;
        _color = color;
        _timer = GameTimer.FromTicks(duration + 1);
    }

    public void Update() {
        _timer = _timer.Decrement();
    }

    public void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.DrawString(Art.Font, _displayString, _position + location, _color);
    }
}

public class CircleEffect : IEffect {
    private readonly CircleF _circle;
    private readonly Color _color;

    private GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public CircleEffect(float radius, Vector2 position, Color color, int duration) {
        _circle = new CircleF(position, radius);
        _color = color;
        _timer = GameTimer.FromTicks(duration + 1);
    }

    public void Update() {
        _timer = _timer.Decrement();
    }

    public void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.DrawCircle(_circle with { Position = _circle.Position + location }, 30, _color);
    }
}

public class PolygonEffect : IEffect {
    private readonly Polygon _polygon;
    private readonly Color _color;

    private GameTimer _timer;

    public bool IsExpired => _timer.IsExpired;

    public PolygonEffect(OrientedRectangle rectangle, Color color, int duration) {
        _polygon = new Polygon(rectangle.Points);
        _color = color;
        _timer = GameTimer.FromTicks(duration + 1);
    }

    public void Update() {
        _timer = _timer.Decrement();
    }

    public void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.DrawPolygon(location, _polygon, _color);
    }
}