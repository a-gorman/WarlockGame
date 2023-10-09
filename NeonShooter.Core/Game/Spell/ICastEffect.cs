using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Entity;

namespace NeonShooter.Core.Game.Spell;

internal interface ICastEffect {

    public void OnCast(IEntity caster, Vector2 castDirection);

}