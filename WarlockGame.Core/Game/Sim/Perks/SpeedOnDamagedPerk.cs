using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Perks;

class SpeedOnDamagedPerk: Perk {
    public SpeedOnDamagedPerk() : base(PerkType.SpeedBoostOnDamage) {
    }

    public override void OnChosen(int forceId, Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockByForceId(forceId);
        if (warlock == null) return;

        warlock.OnDamaged += args => {
            warlock.AddBuff(new SpeedBoost(SimTime.OfSeconds(2)));
        };
    }
}

internal class SpeedBoost : Buff {
    public SpeedBoost(SimTime? duration) : base(BuffType.SpeedBoost, duration) {
        
    }

    public override void OnAdd(Warlock target) {
        target.Speed *= 4;
    }

    public override void OnRemove(Warlock target) {
        target.Speed /= 4;
    }
}