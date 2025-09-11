using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.UI.Components;

sealed class SpellIcon : InterfaceComponent {
    private readonly WarlockSpell _spell;
    private readonly string _hotkey;
    public SpellIcon(WarlockSpell spell, string hotkey) {
        _spell = spell;
        _hotkey = hotkey;
        Clickable = true;
        BoundingBox = spell.SpellIcon.Bounds;
    }

    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.Draw(
            _spell.SpellIcon,
            new Rectangle((int)location.X, (int)location.Y, 50, 50),
            _spell.OnCooldown ? Color.Gray : Color.White);
        spriteBatch.DrawString(Art.Font, _hotkey, location + new Vector2(0, 0), Color.White);
    }

    public override bool OnLeftClick(Vector2 location) {
        InputManager.SelectedSpellId = _spell.Id;
        return true;
    }

    public override bool OnRightClick(Vector2 _) { return true; }
}