using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Graphics;

/// <summary>
/// A sprite that has dedicated rotated versions of the sprite.
/// Rotates by cycling through the rotated versions rather than
/// changing the orientation of a single set of images.
/// </summary>
public class RotatingSprite: ISprite
{
    private readonly Texture2D _image;

    public Vector2 Size { get; }

    public Color Color { get; set; } = Color.White;

    private float _scale;

    private readonly float _baseScale;
    
    private readonly Rectangle[] _sourceRectangles;

    private IdDictionary<float>? _transformations;

    private RotatingSprite(Texture2D image, Rectangle[] sourceRectangles, float baseScale)
    {
        _image = image;
        _sourceRectangles = sourceRectangles;
        Size = sourceRectangles[0].Size.ToVector2();
        _baseScale = baseScale;
        _scale = baseScale;
    }
    
    public static RotatingSprite FromGridSpriteSheet(Texture2D image, int rotations, float scale = 1f)
    {
        return new RotatingSprite(image, image.Bounds.Subdivide(1, rotations).ToArray(), scale);
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Angle orientation, Vector2? origin = null, float opacity = 1) {
        orientation.Radians += float.Pi / _sourceRectangles.Length;
        orientation.WrapPositive();
        var activeSourceRectangle = _sourceRectangles[(int)(orientation.Revolutions * _sourceRectangles.Length)];
        spriteBatch.Draw(_image, position, activeSourceRectangle, Color * opacity, 0, origin ?? Size / 2f, _scale, 0, 0);
    }
    
    public int AddTransformation(float scale) {
        _transformations ??= new IdDictionary<float>();
        var transformationId = _transformations.AddManual(scale);

        _scale = RecalculateScale();
        
        return transformationId;
    }
    
    public bool RemoveTransformation(int transformationId) {
        if (_transformations == null) {
            return false;
        }
        var removed = _transformations.Remove(transformationId);

        if (removed) {
            _scale = RecalculateScale();
        }

        return removed;
    }
    
    public void ChangeTransformation(int transformationId, float value) {
        if (_transformations != null) {
            if (_transformations[transformationId] != value) {
                _transformations[transformationId] = value;
                _scale = RecalculateScale();
            }
        }
    }

    private float RecalculateScale() {
        var scale = _baseScale;
        if (_transformations == null) return scale;
        
        foreach (var transformation in _transformations) {
            scale *= transformation.Value;
        }

        return scale;
    }
}