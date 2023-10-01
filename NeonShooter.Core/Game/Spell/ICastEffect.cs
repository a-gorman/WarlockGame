using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.Spell;

internal interface ICastEffect {

    public void OnCast(Vector2 castPosition, Vector2 castDirection);

}