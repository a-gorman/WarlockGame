using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

sealed class SpellIcon : InterfaceComponent {
    private readonly WarlockSpell _spell;
    private readonly string _hotkey;
    
    public SpellIcon(WarlockSpell spell, string hotkey) {
        _spell = spell;
        _hotkey = hotkey;
        Clickable = ClickableState.Clickable;
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.Draw(
            _spell.Definition.SpellIcon,
            BoundingBox.WithOffset(location),
            _spell.OnCooldown ? Color.Gray : Color.White);
        spriteBatch.DrawString(Art.Font, _hotkey, location + new Vector2(0, 0), Color.White);
    }

    public override void OnLeftClick(Vector2 _) { InputManager.SelectedSpellId = _spell.Id; }
}