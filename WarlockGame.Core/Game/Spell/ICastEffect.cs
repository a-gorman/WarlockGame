using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell;

internal interface ICastEffect {

    public void OnCast(IEntity caster, Vector2 castDirection);

}