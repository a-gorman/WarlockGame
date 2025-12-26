using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellDefinition {
    public required int Id { get; set; }
    public required string Name { get; init; }
    public required SimTime CooldownTime { get; init; }
    public required Texture2D SpellIcon { get; init; }
    public float? CastRange { get; init; }
    public required OneOf<IDirectionalSpellComponent, ILocationSpellComponent, ISelfSpellComponent> Effect { get; init; }
}