using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellDefinition {
    public SpellDefinition(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        GameSound sound,
        params IDirectionalSpellComponent[] effects) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
        Sound = sound;
        Type = SpellType.Directional;
    }

    public SpellDefinition(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        GameSound sound,
        params ILocationSpellComponent[] effects) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
        Sound = sound;
        Type = SpellType.Location;
    }

    public SpellDefinition(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        GameSound sound,
        params ISelfSpellComponent[] effects) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
        Sound = sound;
        Type = SpellType.Self;
    }
    
    public int Id { get; }
    public string Name { get; }
    public SpellType Type { get; }
    public GameSound Sound { get; }
    public SimTime CooldownTime { get; }
    public Texture2D SpellIcon { get; }
    public float? CastRange { get; init; }
    public OneOf<IDirectionalSpellComponent[], ILocationSpellComponent[], ISelfSpellComponent[]> Effects { get; }

    public enum SpellType {
        Directional,
        Location,
        Self
    }
}