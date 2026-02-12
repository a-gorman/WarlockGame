using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Perks;

class PerkManager {
    public event Action<int, Perk>? PerkChosen;
    
    private const int PerkSelections = 3;
    
    private readonly Simulation _sim;
    
    private readonly Dictionary<int, Perk> _perks = new();
    private readonly Dictionary<(int forceId, int perkTypeId), bool> _forcePerks = new();

    private readonly Dictionary<int, List<Perk>> _availablePerks = new();
    
    public PerkManager(Simulation sim) {
        _sim = sim;
    }

    public void Update() {
        foreach (var perk in _perks.Values) {
            perk.Update(_sim);
        }
    }

    public void Initialize() {
        AddPerk(new FlameStrikeSpellPerk(_sim.SpellFactory));
        AddPerk(new ReducedBoundsDamagePerk());
        AddPerk(new ReducedAllDamagePerk());
        AddPerk(new PermanentInvisibilityPerk());
        AddPerk(new PermanentRegenerationPerk());
        AddPerk(new PermanentDamageBoostPerk());
        AddPerk(new PowerFromDamagePerk());
    }

    public List<Perk>? GetAvailablePerks(int forceId) {
        _availablePerks.TryGetValue(forceId, out var selections);
        return selections;
    }

    public void ChoosePerk(int forceId, int perkId) {
        if (_perks.TryGetValue(perkId, out var perk)) {
            _forcePerks[(forceId, perkId)] = true;
            perk.OnAdded(forceId, _sim);
            PerkChosen?.Invoke(forceId, perk);
        } else { 
            Logger.Warning($"Tried to choose perk that does not exist: {perkId}", Logger.LogType.Simulation | Logger.LogType.PlayerAction);  
        }

        ReselectPerksForForce(forceId);
    }

    public void Clear() {
        foreach (var perk in _perks.Values) {
            perk.Clear(_sim);
        }
        
        _forcePerks.Clear();
        
        foreach (var force in _sim.Forces) {
            ReselectPerksForForce(force.Id);
        }
    }
    
    private void ReselectPerksForForce(int forceId) {
        var perks = _perks.Values.ToArray();
        _sim.Random.Shuffle(perks);
        var selections = _availablePerks.GetOrAdd(forceId, _ => []);
        selections.Clear();
        selections.AddRange(perks.Where(x => !HasPerk(forceId, x.Id)).Take(PerkSelections));
    }

    private bool HasPerk(int forceId, int perkId) {
        return _forcePerks.GetValueOrDefault((forceId, perkId), false);
    }

    public void AddPerk(Perk perk) {
        _perks.Add(perk.Id, perk);
    }
}