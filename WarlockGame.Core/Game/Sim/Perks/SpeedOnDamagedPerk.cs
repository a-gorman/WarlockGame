using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Perks;

class SpeedOnDamagedPerk: Perk {
    public SpeedOnDamagedPerk() 
        : base(type: PerkType.SpeedBoostOnDamage,
            name: "Damage Boost",
            description: "Permanently grants you invisibility from distant enemies",
            texture: Art.DefianceIcon) { }

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