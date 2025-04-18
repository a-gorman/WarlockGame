using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.Sim.Buff;

class DamageOverTime : IBuff {
    private readonly SpellContext _spellContext;
    private readonly float _damagePerTick;
    private readonly GameTimer _gameTimer;
    
    public bool IsExpired { get; set; }


    public DamageOverTime(SpellContext spellContext, int durationInTicks, float damagePerTick) {
        _spellContext = spellContext;
        _damagePerTick = damagePerTick;
        _gameTimer = GameTimer.FromTicks(durationInTicks);
    }
    
    public void Update(Warlock target) {
        _gameTimer.Decrement();
        if (_gameTimer.IsExpired) {
            IsExpired = true;
        }
        
        target.Damage(_damagePerTick, _spellContext.Caster);
    }
}