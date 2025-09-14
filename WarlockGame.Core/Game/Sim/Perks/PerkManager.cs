using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Entities;
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
            perk.Update();
        }
    }

    public void AddPerk(Perk perk) {
        perk.Id = _nextPerkId++;
        var currentPerks = _playerPerks.GetOrAdd(perk.ForceId, _ => new());
        currentPerks.TryAdd(perk.Type, perk);
        _perks.Add(perk);
        perk.OnAdd();
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
    public Perk(int forceId, PerkType type, Simulation sim) {
        Type = type;
        ForceId = forceId;
        _sim = sim;
    }
    public int Id { get; set; }
    public PerkType Type { get; set; }
    public int ForceId { get; set; }
    protected readonly Simulation _sim;
    public virtual void Update() { }
    public virtual void OnAdd() { }
    public virtual void OnRemoved() { }

    public enum PerkType {
        Invalid = 0,
        Invisibility
    }
}


class InvisibilityPerk : Perk {
    public InvisibilityPerk(int forceId, Simulation sim) 
        : base(type: PerkType.Invisibility, forceId: forceId, sim: sim) {
    }

    public float Visibility { 
        get; 
        set => field = float.Clamp(value, 0, 1);
    } = 1;

    public const float FadeInDistanceMin = 200;
    public const float FadeInDistanceMax = 300;
    
    private const float VisibilityDecay = 0.006f;

    public float CalculateVisibility(float distance) {
        return distance switch {
            >= FadeInDistanceMax => Visibility,
            > FadeInDistanceMin and < FadeInDistanceMax => 
                float.Lerp(Visibility, 1, (FadeInDistanceMax - distance) / (FadeInDistanceMax - FadeInDistanceMin)),
            <= FadeInDistanceMin => 1,
            float.NaN => 1
        };
    }
    
    public override void Update() {
        Visibility -= VisibilityDecay;
    }
    
    public override void OnAdd() {
        var warlock = _sim.EntityManager.GetWarlockLivingOrDeadByForceId(ForceId);
        if(warlock == null) { return; }

        warlock.SpellCast += HandleWarlockAction;
        warlock.OnDamaged += _ => Visibility = 1;
        warlock.Respawned += HandleWarlockAction;
    }

    public override void OnRemoved() {
        var warlock = _sim.EntityManager.GetWarlockLivingOrDeadByForceId(ForceId);
        if(warlock == null) { return; }
        
        warlock.SpellCast -= HandleWarlockAction;
        warlock.Respawned -= HandleWarlockAction;
    }
    
    private void HandleWarlockAction(Warlock _) { Visibility = 1; }
}