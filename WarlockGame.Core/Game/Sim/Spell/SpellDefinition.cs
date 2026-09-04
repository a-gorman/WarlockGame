using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell;

closed class SpellDefinition {
    public int Id { get; }
    public string Name { get; }
    public GameSound? CastSound { get; }
    public SimTime CooldownTime { get; }
    public Texture2D SpellIcon { get; }

    protected SpellDefinition(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        GameSound? castSound = null) {
        Id = id;
        Name = name;
        CooldownTime = cooldownTime;
        SpellIcon = spellIcon;
        CastSound = castSound;
    }
}
    
class DirectionalSpell: SpellDefinition {
    public IDirectionalSpellComponent[] Effects { get; }

    public DirectionalSpell(
    int id,
    string name,
    SimTime cooldownTime,
    Texture2D spellIcon,
    IDirectionalSpellComponent[] effects,
    GameSound? castSound = null) 
    : base(id, name, cooldownTime, spellIcon, castSound) {
        Effects = effects;
    }
}

class LocationSpell : SpellDefinition {
    public ILocationSpellComponent[] Effects { get; }
    public ISelfSpellComponent[] SelfEffects { get; }

    public float? CastRange { get; }
    
    public LocationSpell(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        ILocationSpellComponent[] effects, 
        float? castRange = null, 
        ISelfSpellComponent[]? selfEffects = null,
        GameSound? castSound = null) : base(id, name, cooldownTime, spellIcon, castSound) {
        Effects = effects;
        SelfEffects = selfEffects ?? [];
        CastRange = castRange;
    }
}

class SelfCastSpell : SpellDefinition {
    public ISelfSpellComponent[] Effects { get; }
    public ILocationSpellComponent[] CastLocationEffects { get; }

    public SelfCastSpell(
        int id,
        string name,
        SimTime cooldownTime,
        Texture2D spellIcon,
        ISelfSpellComponent[] effects,
        ILocationSpellComponent[]? castLocationEffects = null,
        GameSound? castSound = null)
        : base(id, name, cooldownTime, spellIcon, castSound) {
        Effects = effects;
        CastLocationEffects = castLocationEffects ?? [];
    }
}
