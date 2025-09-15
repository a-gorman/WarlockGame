using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.Sim.Buff;

class DamageOverTime : IBuff {
    private readonly SpellContext _spellContext;
    private readonly float _damagePerTick;
    private GameTimer _gameTimer;

    public int Id { get; set; }
    public IBuff.BuffType Type => IBuff.BuffType.DamageOverTime;
    public bool IsExpired { get; set; }


    public DamageOverTime(SpellContext spellContext, SimTime timer, float damagePerTick) {
        _spellContext = spellContext;
        _damagePerTick = damagePerTick;
        _gameTimer = timer.ToTimer();
    }
    
    public void Update(Warlock target) {
        _gameTimer = _gameTimer.Decrement();
        if (_gameTimer.IsExpired) {
            IsExpired = true;
        }
        
        target.Damage(_damagePerTick, _spellContext.Caster);
    }
}