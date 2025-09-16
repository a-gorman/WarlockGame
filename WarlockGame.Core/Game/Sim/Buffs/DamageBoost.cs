using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class DamageBoost : Buff {
    private readonly float _multiplier;

    public DamageBoost(float multiplier, SimTime? duration) : base(BuffType.DamageBoost, duration) {
        _multiplier = multiplier;
    }

    public override void OnAdd(Warlock target) {
        target.DamageMultiplier *= _multiplier;
    }

    public override void OnRemove(Warlock target) {
        target.DamageMultiplier /= _multiplier;
    }
}