using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Spell;

internal interface ISpell
{
    int ManaCost { get; }

    void Update();

    GameTimer Cooldown { get; }
    
    bool OnCooldown { get; }
    
    void OnHit();
    
    void Cast(Vector2 position, Vector2 direction);
}