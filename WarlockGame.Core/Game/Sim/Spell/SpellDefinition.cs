using System;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellDefinition {
    public SpellDefinition(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        Action<SpellContext, Vector2>[] effects,
        GameSound? castSound = null) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
        CastSound = castSound;
        Type = SpellType.Directional;
    }
    
    public SpellDefinition(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        IDirectionalSpellComponent[] effects,
        GameSound? castSound = null) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
        CastSound = castSound;
        Type = SpellType.Directional;
    }

    public SpellDefinition(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        ILocationSpellComponent[] effects,
        GameSound? castSound = null) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
        CastSound = castSound;
        Type = SpellType.Location;
    }

    public SpellDefinition(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        ISelfSpellComponent[] effects,
        GameSound? castSound = null) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        Effects = effects;
        CastSound = castSound;
        Type = SpellType.Self;
    }
    
    public int Id { get; }
    public string Name { get; }
    public SpellType Type { get; }
    public GameSound? CastSound { get; }
    public SimTime CooldownTime { get; }
    public Texture2D SpellIcon { get; }
    public float? CastRange { get; init; }
    public OneOf<
        IDirectionalSpellComponent[], 
        ILocationSpellComponent[], 
        ISelfSpellComponent[], 
        Action<SpellContext, Vector2>[]> 
        Effects { get; }

    public enum SpellType {
        Directional,
        Location,
        Self
    }
}