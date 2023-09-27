using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter.Core.Game.Spell;

internal interface ISpell
{
    int ManaCost { get; }
    
    Texture2D SpellIcon { get; }

    void Update();

    GameTimer Cooldown { get; }
    
    bool OnCooldown { get; }
    
    void Cast(Vector2 position, Vector2 direction);
}