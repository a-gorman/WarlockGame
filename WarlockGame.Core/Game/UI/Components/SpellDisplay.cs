using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components; 

sealed class SpellDisplay : InterfaceComponent {
    private Dictionary<InputAction, string> KeyMappings { get; }
    private Components.Basic.Grid IconGrid { get; }

    private const int SpellSpacing = 100;

    private static readonly InputAction[] Actions = [
        InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4, InputAction.Spell5, 
        InputAction.Spell6, InputAction.Spell7, InputAction.Spell8, InputAction.Spell9, InputAction.Spell10
    ];

    public SpellDisplay(Dictionary<Keys, InputAction> keyMappings) {
        KeyMappings = keyMappings.Where(x => Actions.Contains(x.Value)).ToDictionary(x => x.Value, x => x.Key.ToString());
        Layer = 2;
        BoundingBox = new Rectangle(0, -30, 1880, 90);
        IconGrid = new Components.Basic.Grid(55, 20, Actions.Length, SpellSpacing, 1, 70) {
            Clickable = ClickableState.PassThrough
        };
        AddComponent(IconGrid);
        Clickable = ClickableState.PassThrough;
    }

    public void Reset() {
        foreach (var cell in IconGrid.Cells) {
            cell.RemoveAllComponents();
        }
    }

    public override void OnAdd() {
        WarlockGame.Instance.Simulation.SpellManager.SpellAdded += AddSpell;
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        DrawHollowRectangle(spriteBatch, BoundingBox.WithOffset(location), Color.White);
    }

    private static void DrawHollowRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int width = 1) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 2, 2);
        pointTexture.SetData([Color.Red, Color.Blue, Color.White, Color.Green]);
        
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, width), color); // Bottom line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Top, width, rectangle.Height), color);         // Left line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Right, rectangle.Top, width, rectangle.Height), color);        // Right line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, width), color);    // Top line
    }

    private void AddSpell(int playerId, WarlockSpell spell) {
        if (PlayerManager.IsLocal(playerId)) {
            var spellIcon = new SpellIcon(spell, KeyMappings[Actions[spell.SlotLocation]]) { BoundingBox = new Rectangle(0,0,50,50)};
            IconGrid.AddComponent(spellIcon, 0, spell.SlotLocation);
        }
    }
}