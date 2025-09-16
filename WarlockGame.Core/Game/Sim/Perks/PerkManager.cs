using System.Collections.Generic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Perks;

class PerkManager {
    private readonly Simulation _sim;
    
    private int _nextPerkId = 1;
    
    private readonly Dictionary<int, Dictionary<Perk.PerkType, Perk>> _playerPerks = new();
    private readonly List<Perk> _perks = new();
    
    public PerkManager(Simulation sim) {
        _sim = sim;
    }

    public void Update() {
        foreach (var perk in _perks) {
            perk.Update(_sim);
        }
    }

    public void AddPerk(Perk perk) {
        perk.Id = _nextPerkId++;
        var currentPerks = _playerPerks.GetOrAdd(perk.ForceId, _ => new());
        currentPerks.TryAdd(perk.Type, perk);
        _perks.Add(perk);
        perk.OnAdd(_sim);
    }
    
    public Perk? GetPlayerPerk(int forceId, Perk.PerkType perkTypeId) {
        if (_playerPerks.TryGetValue(forceId, out var perks)
            && perks.TryGetValue(perkTypeId, out var perk)) {
            return perk;
        }
        return null;
    }
}

class Perk {
    public Perk(int forceId, PerkType type) {
        Type = type;
        ForceId = forceId;
    }
    public int Id { get; set; }
    public PerkType Type { get; set; }
    public int ForceId { get; set; }
    public virtual void Update(Simulation sim) { }
    public virtual void OnAdd(Simulation sim) { }
    public virtual void OnRemoved(Simulation sim) { }

    public enum PerkType {
        Invalid = 0,
        Invisibility,
        Regeneration,
        DamageBoost
    }
}