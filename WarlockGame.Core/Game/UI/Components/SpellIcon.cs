using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.UI.Components;

class SpellIcon(WarlockSpell spell, string hotkey) : InterfaceComponent {
    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.Draw(
            spell.SpellIcon,
            new Rectangle((int)location.X, (int)location.Y, 50, 50),
            spell.OnCooldown ? Color.Gray : Color.White);
        spriteBatch.DrawString(Art.Font, hotkey, location + new Vector2(0, 0), Color.White);
    }
}