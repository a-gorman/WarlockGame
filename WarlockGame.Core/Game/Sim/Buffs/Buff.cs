using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class Buff {
    public int Id { get; set; }
    public BuffType Type { get; }
    public GameTimer? Timer { get; set; }
    
    public bool ClearedOnDeath { get; set; } = true;
    public bool IsExpired { get; set; }
    public StackingType Stacking { get; set; } = StackingType.Refreshes;
    
    protected Buff(BuffType type, SimTime? duration) {
        Type = type;
        Timer = duration?.ToTimer();
    }
    
    public virtual void Update(Warlock target) {
        Timer = Timer?.Decrement() ?? null;
        IsExpired |= Timer?.IsExpired ?? false;
    }

    public virtual void OnAdd(Warlock target) { }

    public virtual void OnRemove(Warlock target) { }

    public virtual void OnRespawn() { }
    
    public enum BuffType {
        Invalid = 0,
        Invisibility,
        DamageOverTime,
        Regeneration,
        DamageBoost,
        PowerFromDamage,
        Defense,
        Slow,
    }

    internal enum StackingType {
        Invalid = 0,
        Refreshes = 1,
        Stacks = 2
    }
}