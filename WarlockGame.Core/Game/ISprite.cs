using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace WarlockGame.Core.Game;

interface ISprite {
    Color Color { get; set; }
    Vector2 Size { get; }
    void Draw(SpriteBatch spriteBatch, Vector2 position, Angle orientation, Vector2? origin = null, float opacity = 1);
}