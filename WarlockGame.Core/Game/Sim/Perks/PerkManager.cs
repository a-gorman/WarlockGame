using System;
using System.Collections.Generic;
using System.Linq;

namespace WarlockGame.Core.Game.Sim.Perks;

class PerkManager {
    public event Action<int, Perk>? PerkChosen;
    
    private readonly Simulation _sim;
    
    private readonly Dictionary<int, Perk> _perks = new();
    private readonly Dictionary<(int forceId, int perkTypeId), bool> _forcePerks = new();
    
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
        AddPerk(new PermanentInvisibilityPerk());
        AddPerk(new PermanentRegenerationPerk());
        AddPerk(new PermanentDamageBoostPerk());
        AddPerk(new SpeedOnDamagedPerk());
    }

    public IEnumerable<Perk> GetAvailablePerks(int forceId) {
        return _perks.Values.Where(x => !_forcePerks.GetValueOrDefault((forceId, x.Id), false)).Take(3);
    }

    public void ChoosePerk(int forceId, int perkId) {
        if (_perks.TryGetValue(perkId, out var perk)) {
            _forcePerks[(forceId, perkId)] = true;
            perk.OnAdded(forceId, _sim);
            PerkChosen?.Invoke(forceId, perk);
        }
    }

    public void AddPerk(Perk perk) {
        _perks.Add(perk.Id, perk);
    }
}