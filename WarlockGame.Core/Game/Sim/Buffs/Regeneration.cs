using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class Regeneration: Buff {
    private readonly float _regenAmount;
    private readonly SimTime? _damageCooldown;
    private GameTimer _regenTimer;

    public Regeneration(float regenAmount, SimTime? duration, SimTime? damageCooldown) 
        : base(BuffType.Regeneration, duration) {
        _regenAmount = regenAmount;
        _damageCooldown = damageCooldown;
        _regenTimer = GameTimer.Expired();
    }

    public override void OnAdd(Warlock target) {
        if (_damageCooldown != null) {
            target.OnDamaged += HandleDamaged;
        }
    }

    public override void OnRemove(Warlock target) {
        if (_damageCooldown != null) {
            target.OnDamaged -= HandleDamaged;
        }
    }

    private void HandleDamaged(OnDamagedEventArgs args) {
        _regenTimer = _damageCooldown!.Value.ToTimer();
    }

    protected override void OnUpdate(Warlock target) {
        if (!_regenTimer.IsExpired) {
            target.Health += _regenAmount;
        }

        _regenTimer = _regenTimer.Decremented();
    }
}