using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Log;

namespace WarlockGame.Core.Game.Sim.Perks;

class PerkManager {
    public event Action<int, Perk>? PerkChosen;
    
    private readonly Simulation _sim;
    
    private int _nextPerkId = 1;
    
    private readonly Dictionary<PerkType, Perk> _perks = new();
    private readonly Dictionary<(int, PerkType), bool> _forcePerks = new();
    
    public PerkManager(Simulation sim) {
        _sim = sim;
    }

    public void Update() {
        foreach (var perk in _perks.Values) {
            perk.Update(_sim);
        }
    }

    public void Initialize() {
        AddPerk(PerkType.NewSpells);
        AddPerk(PerkType.SpeedBoostOnDamage);
        AddPerk(PerkType.DamageBoost);
        AddPerk(PerkType.Invisibility);
        AddPerk(PerkType.Regeneration);
    }

    public IEnumerable<Perk> GetAvailablePerks(int forceId) {
        return _perks.Values.Where(x => !_forcePerks.GetValueOrDefault((forceId, x.Type), false)).Take(3);
    }

    public void ChoosePerk(int forceId, PerkType perkType) {
        if (_perks.TryGetValue(perkType, out var perk)) {
            _forcePerks[(forceId, perkType)] = true;
            perk.OnAdded(forceId, _sim);
            PerkChosen?.Invoke(forceId, perk);
        }
    }

    public void AddPerk(PerkType perkType) {
        Perk? perk = perkType switch {
            PerkType.Invisibility => new PermanentInvisibilityPerk(),
            PerkType.Regeneration => new PermanentRegenerationPerk(),
            PerkType.DamageBoost => new PermanentDamageBoostPerk(),
            PerkType.SpeedBoostOnDamage => new SpeedOnDamagedPerk(),
            _ => null
        };
        if (perk == null) {
            Logger.Error($"Invalid perk type: {perkType}");
            return;
        }
        
        AddPerk(perk);
    }
    
    public void AddPerk(Perk perk) {
        perk.Id = _nextPerkId++;
        _perks.Add(perk.Type, perk);
    }
}