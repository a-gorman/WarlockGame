using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI; 

/// <summary>
/// Assumes single active player (No local coop)
/// </summary>
public sealed class SpellDisplay : InterfaceComponent {
    public Dictionary<InputAction, string> KeyMappings { get; }

    private const int SpellSpacing = 100;

    private static readonly InputAction[] Actions = [ 
        InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4, InputAction.Spell5, 
        InputAction.Spell6, InputAction.Spell7, InputAction.Spell8, InputAction.Spell9, InputAction.Spell10
    ];

    public SpellDisplay(Dictionary<Keys, InputAction> keyMappings) {
        KeyMappings = keyMappings.ToDictionary(x => x.Value, x => x.Key.ToString());
        Layer = 2;
        BoundingBox = new Rectangle(20, 925, 1880, 90);
    }
    
    public override void OnClick(Vector2 location) {
        Logger.Info("Click the spell display!");
    }
    
    public override void Draw(SpriteBatch spriteBatch) {
        DrawHollowRectangle(spriteBatch, BoundingBox, Color.White);

        var localWarlock = PlayerManager.LocalPlayer?.Id.Let(x => WarlockGame.Instance.Simulation.EntityManager.GetWarlockByPlayerId(x));
        if(localWarlock is null) return;
        
        for (var i = 0; i < localWarlock.Spells.Count; i++) {
            var spell = localWarlock.Spells[i];
            spriteBatch.Draw(
                spell.SpellIcon,
                new Rectangle(60 + SpellSpacing * i, 950, 50, 50),
                spell.OnCooldown ? Color.Gray : Color.White);
            spriteBatch.DrawString(Art.Font, KeyMappings[Actions[i]], new Vector2(55 + SpellSpacing * i, 950-9), Color.White);
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