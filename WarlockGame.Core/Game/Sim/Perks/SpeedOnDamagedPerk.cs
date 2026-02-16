using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Perks;

class PowerFromDamagePerk : Perk {
    public PowerFromDamagePerk()
        : base(
            id: 4,
            name: "Power from damage",
            description: "Damage gives you a temporary power boost",
            texture: Art.DefianceIcon) { }

    public override void OnAdded(int forceId, Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(forceId);
        if (warlock == null) return;

        warlock.OnDamaged += AddBuff;
    }

    private void AddBuff(OnDamagedEventArgs args) {
        (args.Source as Warlock)!.AddBuff(new SpeedBoost(SimTime.OfSeconds(2)));
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
            target.Speed *= 4;
            target.GenericDefense *= 0.5f;
            target.BoundsDefense *= 2f;
            target.DamageMultiplier *= 2f;
        }

        public override void OnRemove(Warlock target) {
            target.Speed /= 4;
            target.GenericDefense /= 0.5f;
            target.BoundsDefense /= 2f;
            target.DamageMultiplier /= 2f;
        }
    }
}