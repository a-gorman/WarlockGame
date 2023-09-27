using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Entity;

namespace NeonShooter.Core.Game.UI; 

public static class SpellDisplay {
    
    private const int spellSpacing = 100;
    
    public static void Draw(SpriteBatch spriteBatch) {
        DrawHollowRectangle(spriteBatch, new Rectangle(20, 925, 1880, 90), Color.White);

        var spellIndex = 0;
        foreach (var spell in PlayerShip.Instance.Spells) {
            spriteBatch.Draw(
                spell.SpellIcon, 
                new Rectangle(60 + spellSpacing * spellIndex, 950, 50, 50), 
                spell.OnCooldown ? Color.Gray : Color.White);
            spellIndex++;
        }
    }

    private static void DrawHollowRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int width = 1) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 2, 2);
        pointTexture.SetData(new[] { Color.Red, Color.Blue, Color.White, Color.Green });
        
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, width), color); // Bottom line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Top, width, rectangle.Height), color);         // Left line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Right, rectangle.Top, width, rectangle.Height), color);        // Right line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, width), color);    // Top line
    }
}