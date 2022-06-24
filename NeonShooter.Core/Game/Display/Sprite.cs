using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter.Core.Game.Display;

public class Sprite
{
    private readonly Texture2D _image;

    public Vector2 Size => new Vector2(ActiveSourceRectangle.Width, ActiveSourceRectangle.Height);

    public Color Color { get; set; } = Color.White;

    public float Scale { get; set; } = 1f;

    private readonly List<Rectangle> _sourceRectangles;

    private int _activeSourceRectangleIndex = 0;

    private readonly int _framesBetweenTransitions = 1;
    
    private int _frameCounter = 1;

    private Rectangle ActiveSourceRectangle => _sourceRectangles[_activeSourceRectangleIndex];

    public Sprite(Texture2D image)
    {
        _image = image;
        _sourceRectangles = new List<Rectangle>()
        {
            image.Bounds
        };
    }

    private Sprite(Texture2D image, List<Rectangle> sourceRectangles, int framesBetweenTransitions)
    {
        _image = image;
        _sourceRectangles = sourceRectangles;
        _framesBetweenTransitions = framesBetweenTransitions;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float orientation)
    {
        spriteBatch.Draw(_image, position, ActiveSourceRectangle, Color, orientation, Size / 2f, Scale, 0, 0);
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
        if (_activeSourceRectangleIndex == _sourceRectangles.Count - 1)
        {
            _activeSourceRectangleIndex = 0;
        }
        else
        {
            _activeSourceRectangleIndex++;
        }
    }

    public static Sprite FromGridSpriteSheet(Texture2D image, int subdivisionsX, int subdivisionsY, int framesBetweenTransitions)
    {
        return new Sprite(image, image.Bounds.Subdivide(subdivisionsX, subdivisionsY).ToList(), framesBetweenTransitions);
    }
}