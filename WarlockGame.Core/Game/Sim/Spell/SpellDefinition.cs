using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellDefinition {
    public SpellDefinition(
        int id, string name, SimTime cooldownTime, Texture2D spellIcon, params IDirectionalSpellComponent[] effects) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
    }
    
    public SpellDefinition(int id, string name, SimTime cooldownTime, Texture2D spellIcon, params ILocationSpellComponent[] effects) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
    }
    
    public SpellDefinition(int id, string name, SimTime cooldownTime, Texture2D spellIcon, params ISelfSpellComponent[] effects) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
    }
    
    public int Id { get; private init; }
    public string Name { get; private init; }
    public SimTime CooldownTime { get; private init; }
    public Texture2D SpellIcon { get; private init; }
    public float? CastRange { get; init; }
    public OneOf<IDirectionalSpellComponent[], ILocationSpellComponent[], ISelfSpellComponent[]> Effects { get; private init; }
}