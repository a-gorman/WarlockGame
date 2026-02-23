using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellManager {
    private readonly SpellFactory _spellFactory;

    private int _nextSpellId = 1;
    
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
            _spellFactory.DeflectionShield(),
            _spellFactory.Homing(),
            _spellFactory.Boomerang(),
            _spellFactory.FlameStrike(),
            _spellFactory.FireSpray(),
            _spellFactory.Blink(),
            _spellFactory.Shockwave()
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

        WarlockSpell spell = _spellFactory.CreateWarlockSpell(definition, _nextSpellId++);
        var spellbook = PlayerSpells.GetOrAdd(forceId, _ => new Dictionary<int, WarlockSpell>());
        spellbook.Add(spell.Definition.Id, spell);
        Spells[spell.Id] = spell;
        spell.SlotLocation = spellbook.Count - 1;
        
        Logger.Info($"Added spell {spell.Definition.Name} with id {spell.Id} at slot {spell.SlotLocation} to force {forceId}", Logger.LogType.Simulation);
        
        SpellAdded?.Invoke(forceId, spell);
    }

    public void RemoveSpell(int forceId, int definitionId) {
        var removed = PlayerSpells.TryGetValue(forceId, out var spellBook) && spellBook.Remove(definitionId);
        if (!removed) {
            Logger.Warning($"Tried removing a spell from a player that does not have that spell. Force: {forceId} Definition: {definitionId}", Logger.LogType.Simulation);
        }
    }

    public WarlockSpell? GetSpell(int forceId, int definitionId) {
        if (PlayerSpells.TryGetValue(forceId, out var spellBook) && spellBook.TryGetValue(definitionId, out var spell))
            return spell;
        return null;
    }

    public void Clear() {
        PlayerSpells.Clear();
        Spells.Clear();
        _nextSpellId = 1;
    }
}