using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellManager {
    private readonly SpellFactory _spellFactory;
    
    public Dictionary<int, SpellDefinition> Definitions { get; set; }

    // Force -> SpellTypeId -> Spell
    public Dictionary<int, Dictionary<int, WarlockSpell>> PlayerSpells { get; } = [];
    // Spell instance's id -> Spell
    public Dictionary<int, WarlockSpell> Spells { get; } = [];

    public delegate void SpellAddedHandler(int playerId, WarlockSpell warlockSpell);
    public event SpellAddedHandler? SpellAdded;

    public SpellManager(SpellFactory spellFactory) {
        _spellFactory = spellFactory;
        Definitions = new[] {
            _spellFactory.Fireball(),
            _spellFactory.Lightning(),
            _spellFactory.Poison(),
            _spellFactory.Burst(),
            _spellFactory.WindShield(),
            _spellFactory.SoulShatter(),
            _spellFactory.RefractionShield(),
            _spellFactory.Homing(),
            _spellFactory.Boomerang()
        }.ToDictionary(x => x.Id, x => x);
    }
    
    public void Update() {
        foreach (var spell in Spells.Values) {
            spell.Update();
        }
    }

    public void AddSpell(int forceId, int definitionId) {
        if (!Definitions.TryGetValue(definitionId, out var definition)) {
            Logger.Error($"Tried to add spell type that does not exist! Force: {forceId} Definition {definitionId}", Logger.LogType.Simulation);
            return;
        }

        if (GetSpell(forceId, definitionId) != null) {
            Logger.Warning($"Tried adding a spell that warlock already has! Force: {forceId} Definition: {definition}", Logger.LogType.Simulation);
            return;
        }

        AddSpell(forceId, _spellFactory.CreateWarlockSpell(definition));
    }
    
    public void AddSpell(int forceId, WarlockSpell spell) {
        var spellbook = PlayerSpells.GetOrAdd(forceId, _ => new Dictionary<int, WarlockSpell>());
        spellbook.Add(spell.Definition.Id, spell);
        Spells[spell.Id] = spell;
        spell.SlotLocation = spellbook.Count - 1;
        SpellAdded?.Invoke(forceId, spell);
    }

    public WarlockSpell? GetSpell(int forceId, int definitionId) {
        if (PlayerSpells.TryGetValue(forceId, out var spellBook) && spellBook.TryGetValue(definitionId, out var spell))
            return spell;
        return null;
    }

    public void Clear() {
        PlayerSpells.Clear();
    }
}