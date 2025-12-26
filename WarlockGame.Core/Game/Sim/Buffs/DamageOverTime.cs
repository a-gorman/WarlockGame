using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.Sim.Buffs;

class DamageOverTime : Buff {
    private readonly SpellContext _spellContext;
    private readonly float _damagePerTick;
    
    public DamageOverTime(SpellContext spellContext, SimTime duration, float damagePerTick) : base(BuffType.DamageOverTime, duration) {
        _spellContext = spellContext;
        _damagePerTick = damagePerTick;
    }
    
    public override void Update(Warlock target) {
        target.Damage(_damagePerTick, DamageType.Player, _spellContext.Caster);
        base.Update(target);
    }
}