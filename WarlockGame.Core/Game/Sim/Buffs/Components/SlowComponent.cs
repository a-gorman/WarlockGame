using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class SlowComponent: BuffComponent {
    private readonly float _factor;
    
    public SlowComponent(float factor) {
        _factor = factor;
    }

    public override void OnAdd(Buff buff, Warlock target) {
        target.Speed *= _factor;
    }

    public override void OnRemove(Buff buff, Warlock target) {
        target.Speed /= _factor;
    }
}