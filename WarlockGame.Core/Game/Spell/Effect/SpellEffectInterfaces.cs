using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Spell.Effect;

/// <summary>
/// Effect that is applied towards a direction, such as spawning a fireball
/// </summary>
interface IDirectionalSpellEffect {
    public void Invoke(Warlock caster, Vector2 castLocation, Vector2 invokeDirection);
}

/// <summary>
/// Effect that applies at a location, such as an explosion
/// </summary>
interface ILocationSpellEffect {
    public void Invoke(Warlock caster, Vector2 invokeLocation);
}

/// <summary>
/// Effect that applies only to the caster, such as shielding oneself
/// </summary>
interface ISelfSpellEffect {
    public void Invoke(Warlock caster);
}

interface IWarlockEffect {
    void Invoke(Warlock caster, IReadOnlyCollection<TargetInfo> targets);
}