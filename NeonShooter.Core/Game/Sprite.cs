using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter.Core.Game;

public class Sprite
{
    private Texture2D _image;
    
    public Vector2 Size { get; }

    public Color Color { get; set; } = Color.White;
    
    public float Scale { get; set; } = 1f;

    private Rectangle? sourceRectangle;

    public Sprite(Texture2D image)
    {
        _image = image;
        Size = new Vector2(_image.Width, _image.Height);
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, float orientation)
    {
        spriteBatch.Draw(_image, position, null, Color, orientation, Size / 2f, Scale, 0, 0);
    }
}