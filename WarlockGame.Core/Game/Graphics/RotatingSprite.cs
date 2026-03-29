using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Graphics;

public class RotatingSprite: ISprite
{
    private readonly Texture2D _image;

    public Vector2 Size { get; }

    public Color Color { get; set; } = Color.White;

    public float Scale { get; set; } = 1f;

    private readonly Rectangle[] _sourceRectangles;

    private RotatingSprite(Texture2D image, Rectangle[] sourceRectangles)
    {
        _image = image;
        _sourceRectangles = sourceRectangles;
        Size = sourceRectangles[0].Size.ToVector2();
    }
    
    // 0,          1,           2,        3
    // -1/4-1/4    1/4-3/4    3/4-5/4    5/4-7/4
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Angle orientation, Vector2? origin = null, float opacity = 1) {
        orientation.Radians += float.Pi / _sourceRectangles.Length;
        orientation.WrapPositive();
        var activeSourceRectangle = _sourceRectangles[(int)(orientation.Revolutions * _sourceRectangles.Length)];
        spriteBatch.Draw(_image, position, activeSourceRectangle, Color * opacity, 0, origin ?? Size / 2f, Scale, 0, 0);
    }

    public static RotatingSprite FromGridSpriteSheet(Texture2D image, int rotations, float scale = 1f)
    {
        var sprite = new RotatingSprite(image, image.Bounds.Subdivide(1, rotations).ToArray()) {
            Scale = scale
        };
        return sprite;
    }
}