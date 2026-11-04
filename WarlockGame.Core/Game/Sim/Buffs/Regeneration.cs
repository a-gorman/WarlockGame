using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class Regeneration: Buff {
    private readonly float _regenAmount;
    private readonly SimTime? _damageDisableTime;
    private GameTimer? _regenDelay;

    public Regeneration(float regenAmount, SimTime? duration, SimTime? damageDisableTime) 
        : base(BuffType.Regeneration, duration) {
        _regenAmount = regenAmount;
        _damageDisableTime = damageDisableTime;
    }
    
    protected override void OnUpdate(Warlock target) {
        if (!_regenDelay?.IsExpired ?? true) {
            target.Health += _regenAmount;
        }

        _regenDelay = _regenDelay?.Decremented();
    }
}