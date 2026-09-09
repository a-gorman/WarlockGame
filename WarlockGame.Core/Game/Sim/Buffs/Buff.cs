using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class Buff {
    public int Id { get; set; }
    public BuffType Type { get; }
    public GameTimer? Timer { get; set; }
    
    public bool ClearedOnDeath { get; set; } = true;
    public bool IsExpired { get; set; }
    public StackingType Stacking { get; set; } = StackingType.Refreshes;

    private readonly BuffComponent[] _components;
    
    public Buff(BuffType type, SimTime? duration, BuffComponent[] components, StackingType stacking = StackingType.Stacks) {
        Type = type;
        Timer = duration?.ToTimer();
        _components = components;
    }

    protected Buff(BuffType type, SimTime? duration) {
        Type = type;
        Timer = duration?.ToTimer();
        _components = [];
    }
    
    public void Update(Warlock target) {
        Timer = Timer?.Decremented() ?? null;
        IsExpired |= Timer?.IsExpired ?? false;
        
        foreach(var component in _components) {
            component.update(this, target);
        }

        OnUpdate(target);
    }

    protected virtual void OnUpdate(Warlock target) { }

    public virtual void OnAdd(Warlock target) {
        foreach(var component in _components) {
            component.OnAdd(this, target);
        }
     }

    public virtual void OnRemove(Warlock target) { 
        foreach(var component in _components) {
            component.OnRemove(this, target);
        }
    }

    public virtual void OnRespawn() {
        foreach(var component in _components) {
            component.OnRespawn(this);
        }
     }
    
    public enum BuffType {
        Invalid = 0,
        Invisibility,
        DamageOverTime,
        Regeneration,
        DamageBoost,
        PowerFromDamage,
        Defense,
        Slow,
        Jumping,
    }

    internal enum StackingType {
        Invalid = 0,
        Refreshes = 1,
        Stacks = 2,
        None = 3
    }
}