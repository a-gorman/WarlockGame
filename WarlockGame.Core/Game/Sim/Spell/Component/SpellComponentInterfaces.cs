using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

/// <summary>
/// Effect that is applied towards a direction, such as spawning a fireball
/// </summary>
interface IDirectionalSpellComponent {
    public void Invoke(SpellContext context, Vector2 castLocation, Vector2 invokeDirection);
}

/// <summary>
/// Effect that applies at a location, such as an explosion
/// </summary>
interface ILocationSpellComponent {
    public void Invoke(SpellContext context, Vector2 invokeLocation);
}

/// <summary>
/// Effect that applies only to the caster, such as shielding oneself
/// </summary>
interface ISelfSpellComponent {
    public void Invoke(SpellContext context);
}

/// <summary>
/// Component that applies to warlocks, such as doing damage
/// </summary>
interface IEntityComponent {
    void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets);
}