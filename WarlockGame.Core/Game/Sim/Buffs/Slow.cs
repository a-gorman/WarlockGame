using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class Slow : Buff {
    private readonly float _factor;
    public Slow(float factor, SimTime? duration) : base(BuffType.Slow, duration) {
        _factor = factor;
    }

    public override void OnAdd(Warlock target) {
        // Note: this can accumulate floating point errors
        target.Speed *= _factor;
    }

    public override void OnRemove(Warlock target) {
        target.Speed /= _factor;
    }
}