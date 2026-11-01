using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Graphics;

class Sprite: ISprite
{
    private readonly Texture2D _image;

    public Vector2 Size => new Vector2(ActiveSourceRectangle.Width, ActiveSourceRectangle.Height) * Scale;

    public Color Color { get; set; } = Color.White;

    public float Opacity { get; set; } = 1;
    
    public float Scale { get; private set; }
    public bool Rotates { get; set; } = true;

    private readonly Rectangle[] _sourceRectangles;
    private int _activeSourceRectangleIndex = 0;

    private readonly int _framesBetweenTransitions = 1;
    private int _frameCounter = 1;

    private bool _invisible = false;
    
    private Rectangle ActiveSourceRectangle => _sourceRectangles[_activeSourceRectangleIndex];
    
    private readonly float _baseScale;
    private readonly ExpiredSpriteBehavior _expiredSpriteBehavior;

    private IdDictionary<float>? _transformations;

    public Sprite(Texture2D image, float scale = 1)
    {
        _image = image;
        _sourceRectangles = [image.Bounds];
        Scale = scale;
        _baseScale = scale;
        _expiredSpriteBehavior = ExpiredSpriteBehavior.Hold;
    }

    private Sprite(Texture2D image, Rectangle[] sourceRectangles, int framesBetweenTransitions, float scale, 
        ExpiredSpriteBehavior expiredSpriteBehavior, float opacity)
    {
        _image = image;
        _sourceRectangles = sourceRectangles;
        _framesBetweenTransitions = framesBetweenTransitions;
        Scale = scale;
        Opacity = opacity;
        _baseScale = scale;
        _expiredSpriteBehavior = expiredSpriteBehavior;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Angle orientation, Vector2? origin = null, float opacity = 1) {
        if (_invisible) return;
        if (!Rotates) orientation = new Angle(0);

        spriteBatch.Draw(
            texture: _image,
            position: position,
            sourceRectangle: ActiveSourceRectangle,
            color: Color * opacity * Opacity,
            rotation: orientation,
            origin: origin ?? new Vector2(ActiveSourceRectangle.Width, ActiveSourceRectangle.Height) / 2,
            scale: Scale,
            effects: 0,
            layerDepth: 0);

        AdvanceSpriteFrame();
    }

    public int AddTransformation(float scale = 1) {
        _transformations ??= new IdDictionary<float>();
        var transformationId = _transformations.AddManual(scale);

        Scale = RecalculateScale();
        
        return transformationId;
    }
    
    public bool RemoveTransformation(int transformationId) {
        if (_transformations == null) {
            return false;
        }
        var removed = _transformations.Remove(transformationId);

        if (removed) {
            Scale = RecalculateScale();
        }

        return removed;
    }
    
    public void ChangeTransformation(int transformationId, float value) {
        if (_transformations != null) {
            if (_transformations[transformationId] != value) {
                _transformations[transformationId] = value;
                Scale = RecalculateScale();
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

    private void AdvanceSpriteFrame() {
        if (_frameCounter == _framesBetweenTransitions) {
            _frameCounter = 1;
            if (_activeSourceRectangleIndex != _sourceRectangles.Length - 1) {
                _activeSourceRectangleIndex++;
            } else {
                switch (_expiredSpriteBehavior) {
                    case ExpiredSpriteBehavior.Loop:
                        _activeSourceRectangleIndex = 0;
                        break;
                    case ExpiredSpriteBehavior.Invisible:
                        _invisible = true;
                        break;
                    case ExpiredSpriteBehavior.Hold:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        } else {
            _frameCounter++;
        }
    }

    public static Sprite FromGridSpriteSheet(
        Texture2D image,
        int subdivisionsX,
        int subdivisionsY,
        SimTime loopTime,
        float scale = 1f,
        bool rotates = true,
        float opacity = 1f,
        ExpiredSpriteBehavior expiredSpriteBehavior = ExpiredSpriteBehavior.Loop) {
        
        var sprite = new Sprite(image, 
            image.Bounds.Subdivide(subdivisionsX, subdivisionsY).ToArray(),
            loopTime.Ticks / (subdivisionsX * subdivisionsY),
            scale,
            expiredSpriteBehavior,
            opacity) {
            Rotates = rotates
        };
        return sprite;
    }
}