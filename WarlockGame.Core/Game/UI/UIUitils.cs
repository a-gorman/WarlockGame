using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.UI;

static class UIUitils {
    public static void DrawHollowRectangle(SpriteBatch spriteBatch, Texture2D texture, Rectangle rectangle, Color color, int width = 1) {
        spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, width), color); // Bottom line
        spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Top, width, rectangle.Height), color);         // Left line
        spriteBatch.Draw(texture, new Rectangle(rectangle.Right, rectangle.Top, width, rectangle.Height), color);        // Right line
        spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, width), color);    // Top line
    }
}