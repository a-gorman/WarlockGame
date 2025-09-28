using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellManager {
    private readonly SpellFactory _spellFactory;

    // PlayerId -> SpellTypeId -> Spell
    public Dictionary<int, Dictionary<int, WarlockSpell>> PlayerSpells { get; } = [];
    // Spell instance's id -> Spell
    public Dictionary<int, WarlockSpell> Spells { get; } = [];

    public delegate void SpellAddedHandler(int playerId, WarlockSpell warlockSpell);
    public event SpellAddedHandler? SpellAdded;

    public SpellManager(SpellFactory spellFactory) {
        _spellFactory = spellFactory;
    }
    
    public void Update() {
        foreach (var spell in Spells.Values) {
            spell.Update();
        }
    }

    public void AddSpell(int forceId, int spellTypeId) {
        var spell = spellTypeId switch {
            1 => _spellFactory.Fireball(),
            2 => _spellFactory.Lightning(),
            3 => _spellFactory.Poison(),
            4 => _spellFactory.Burst(),
            5 => _spellFactory.WindShield(),
            6 => _spellFactory.SoulShatter(),
            7 => _spellFactory.RefractionShield(),
            8 => _spellFactory.Homing(),
            9 => _spellFactory.Boomerang(),
            _ => throw new ArgumentOutOfRangeException(nameof(spellTypeId), spellTypeId, null)
        };
        
        AddSpell(forceId, spell);
    }
    
    public void AddSpell(int forceId, WarlockSpell spell) {
        var spellbook = PlayerSpells.GetOrAdd(forceId, _ => new Dictionary<int, WarlockSpell>());
        spellbook.Add(spell.SpellTypeId, spell);
        Spells[spell.Id] = spell;
        spell.SlotLocation = spellbook.Count - 1;
        SpellAdded?.Invoke(forceId, spell);
    }

    public void Clear() {
        PlayerSpells.Clear();
    }
}