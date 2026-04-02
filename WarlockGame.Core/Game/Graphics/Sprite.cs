using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Graphics;

public class Sprite: ISprite
{
    private readonly Texture2D _image;

    public Vector2 Size => new Vector2(ActiveSourceRectangle.Width, ActiveSourceRectangle.Height);

    public Color Color { get; set; } = Color.White;

    public float Scale { get; set; } = 1f;
    public bool Rotates { get; set; } = true;

    private readonly Rectangle[] _sourceRectangles;
    private int _activeSourceRectangleIndex = 0;

    private readonly int _framesBetweenTransitions = 1;
    private int _frameCounter = 1;

    private Rectangle ActiveSourceRectangle => _sourceRectangles[_activeSourceRectangleIndex];

    public Sprite(Texture2D image, float scale = 1)
    {
        _image = image;
        _sourceRectangles = [image.Bounds];
        Scale = scale;
    }

    private Sprite(Texture2D image, Rectangle[] sourceRectangles, int framesBetweenTransitions)
    {
        _image = image;
        _sourceRectangles = sourceRectangles;
        _framesBetweenTransitions = framesBetweenTransitions;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Angle orientation, Vector2? origin = null, float opacity = 1) {
        if (!Rotates) orientation = new Angle(0);
        
        spriteBatch.Draw(_image, position, ActiveSourceRectangle, Color * opacity, orientation, origin ?? Size / 2f, Scale, 0, 0);
        AdvanceSpriteFrame();
    }

    private void AdvanceSpriteFrame()
    {
        if (_frameCounter == _framesBetweenTransitions)
        {
            _frameCounter = 1;
            NextSpriteFrame();
        }
        else
        {
            _frameCounter++;
        }
    }
    
    private void NextSpriteFrame()
    {
        if (_activeSourceRectangleIndex == _sourceRectangles.Length - 1)
        {
            _activeSourceRectangleIndex = 0;
        }
        else
        {
            _activeSourceRectangleIndex++;
        }
    }

    public static Sprite FromGridSpriteSheet(Texture2D image,
        int subdivisionsX,
        int subdivisionsY,
        SimTime timeBetweenTransitions,
        float scale = 1f,
        bool rotates = true)
    {
        var sprite = new Sprite(image, 
            image.Bounds.Subdivide(subdivisionsX, subdivisionsY).ToArray(),
            timeBetweenTransitions.Ticks) {
            Scale = scale,
            Rotates = rotates
        };
        return sprite;
    }
}