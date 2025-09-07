using System.Collections.Generic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellManager {
    // PlayerId -> SpellTypeId -> Spell
    public Dictionary<int, Dictionary<int, WarlockSpell>> PlayerSpells { get; } = [];
    // Spell instance's id -> Spell
    public Dictionary<int, WarlockSpell> Spells { get; } = [];

    public delegate void SpellAddedHandler(int playerId, WarlockSpell warlockSpell);
    public event SpellAddedHandler? SpellAdded;
    
    public void Update() {
        foreach (var spell in Spells.Values) {
            spell.Update();
        }
    }

    public void AddSpell(int playerId, WarlockSpell spell) {
        var spellbook = PlayerSpells.GetOrAdd(playerId, _ => new Dictionary<int, WarlockSpell>());
        spellbook.Add(spell.SpellTypeId, spell);
        Spells[spell.Id] = spell;
        spell.SlotLocation = spellbook.Count - 1;
        SpellAdded?.Invoke(playerId, spell);
    }

    public void Clear() {
        PlayerSpells.Clear();
    }
}