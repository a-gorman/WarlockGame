using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.UI.Components;

class SpellIcon(WarlockSpell spell, string hotkey) : InterfaceComponent {
    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.Draw(
            spell.SpellIcon,
            new Rectangle(0, 0, 50, 50),
            spell.OnCooldown ? Color.Gray : Color.White);
        spriteBatch.DrawString(Art.Font, hotkey, new Vector2(0, 9), Color.White);
    }
}