using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI; 

/// <summary>
/// Assumes single active player (No local coop)
/// </summary>
public class SpellDisplay : IClickableComponent {
    // Spell display never goes away
    public bool IsExpired => false;
    public bool Visible { get; set; } = true;
    public IEnumerable<IInterfaceComponent> Components { get; } = new List<IInterfaceComponent>();

    private const int spellSpacing = 100;

    private static InputAction[] _actions = { InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4, InputAction.Spell5, InputAction.Spell6 };

    public int Layer => 2;
    public Rectangle BoundingBox { get; } = new Rectangle(20, 925, 1880, 90);

    public void OnClick(Vector2 location) {
        Logger.Info("Click the spell display!");
    }

    public List<IClickableComponent> ClickableComponents { get; set; }

    public void Draw(SpriteBatch spriteBatch) {
        DrawHollowRectangle(spriteBatch, BoundingBox, Color.White);

        var localWarlock = PlayerManager.LocalPlayer?.Id.Let(EntityManager.GetWarlockByPlayerId);
        if(localWarlock is null) return;
        
        for (var i = 0; i < localWarlock.Spells.Count; i++) {
            var spell = localWarlock.Spells[i];
            spriteBatch.Draw(
                spell.SpellIcon,
                new Rectangle(60 + spellSpacing * i, 950, 50, 50),
                spell.OnCooldown ? Color.Gray : Color.White);
            spriteBatch.DrawString(Art.Font, StaticKeyboardInput._mappings[_actions[i]].DisplayValue, new Vector2(55 + spellSpacing * i, 950-9), Color.White);
        }
    }

    private static void DrawHollowRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int width = 1) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 2, 2);
        pointTexture.SetData(new[] { Color.Red, Color.Blue, Color.White, Color.Green });
        
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, width), color); // Bottom line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Top, width, rectangle.Height), color);        // Left line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Right, rectangle.Top, width, rectangle.Height), color);       // Right line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, width), color);    // Top line
    }
}