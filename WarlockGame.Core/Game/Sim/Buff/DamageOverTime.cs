using WarlockGame.Core.Game.Sim.Entity;

namespace WarlockGame.Core.Game.Sim.Buff;

class DamageOverTime : IBuff {
    private readonly Warlock _caster;
    private readonly float _damagePerTick;
    private readonly GameTimer _gameTimer;
    
    public bool IsExpired { get; set; }


    public DamageOverTime(Warlock caster, int durationInTicks, float damagePerTick) {
        _caster = caster;
        _damagePerTick = damagePerTick;
        _gameTimer = GameTimer.FromTicks(durationInTicks);
    }
    
    public void Update(Warlock target) {
        _gameTimer.Update();
        if (_gameTimer.IsExpired) {
            IsExpired = true;
        }
        
        target.Damage(_damagePerTick, _caster);
    }
}