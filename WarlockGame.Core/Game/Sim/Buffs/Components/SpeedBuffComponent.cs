using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class SpeedBuffComponent: BuffComponent {
    private readonly float _moveSpeedFactor;
    private readonly float _turnSpeedFactor;

    public SpeedBuffComponent(float moveSpeedFactor, float turnSpeedFactor) {
        _moveSpeedFactor = moveSpeedFactor;
        _turnSpeedFactor = turnSpeedFactor;
    }

    public override void OnAdd(Buff buff, Warlock target) {
        target.Speed *= _moveSpeedFactor;
        target.RotationSpeed *= _turnSpeedFactor;
    }

    public override void OnRemove(Buff buff, Warlock target) {
        target.Speed /= _moveSpeedFactor;
        target.RotationSpeed /= _turnSpeedFactor;
    }
}