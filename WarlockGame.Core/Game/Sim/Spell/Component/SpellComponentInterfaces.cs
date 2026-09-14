using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

/// <summary>
/// Effect that is applied towards a direction, such as spawning a fireball
/// </summary>
interface IDirectionalSpellComponent {
    public void Invoke(SpellContext context, Vector2 invokeLocation, Vector2 invokeDirection);
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
/// Component that applies to a single target, such as doing damage
/// </summary>
interface IEntitySpellComponent {
    void Invoke(SpellContext context, Entity target);
}

/// <summary>
/// Component that applies to a group of targets in an Area of effect, such as 3 warlocks caught in an explosion
/// </summary>
interface IAoeTargetsSpellComponent {
    void Invoke(SpellContext context, IReadOnlyCollection<AoeTargetInfo> targets);
}