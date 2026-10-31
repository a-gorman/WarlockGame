using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Perks;

class PowerFromDamagePerk : Perk {
    private const float Speed = 4;
    private const float GenericDefense = 0.5f;
    private const float BoundsDefense = 2f;
    private const float DamageMultiplier = 2f;
    private const float DurationSeconds = 2f;
    
    
    public PowerFromDamagePerk()
        : base(
            id: 4,
            name: "Power from damage",
            description: $"Whenever you take damage, you get a {Speed:0.#}x speed boost, {DamageMultiplier:0.#}x damage boost and {1 - GenericDefense:%} damage reduction for {DurationSeconds:0.#} seconds. Damage reduction does not work on bounds damage.",
            texture: Art.DefianceIcon) { }

    public override void OnAdded(int forceId, Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(forceId);
        if (warlock == null) return;

        warlock.OnDamaged += AddBuff;
    }

    private void AddBuff(OnDamagedEventArgs args) {
        (args.DamagedEntity as Warlock)!.AddBuff(new SpeedBoost(SimTime.OfSeconds(DurationSeconds)));
    }

    public override void OnRemoved(int forceId, Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(forceId);
        if (warlock == null) return;
        
        warlock.OnDamaged -= AddBuff;
    }

    public override void Clear(Simulation sim) {
        foreach (var force in sim.Forces) {
            OnRemoved(force.Id, sim);
        }
    }

    private class SpeedBoost : Buff {
        public SpeedBoost(SimTime? duration) : base(BuffType.PowerFromDamage, duration) {
            Stacking = StackingType.Refreshes;
        }
        
        public override void OnAdd(Warlock target) {
            target.Speed *= Speed;
            target.GenericDefense *= GenericDefense;
            target.BoundsDefense *= BoundsDefense;
            target.DamageMultiplier *= DamageMultiplier;
        }

        public override void OnRemove(Warlock target) {
            target.Speed /= Speed;
            target.GenericDefense /= GenericDefense;
            target.BoundsDefense /= BoundsDefense;
            target.DamageMultiplier /= DamageMultiplier;
        }
    }
}