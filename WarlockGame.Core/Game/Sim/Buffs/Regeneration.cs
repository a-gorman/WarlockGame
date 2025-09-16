using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class Regeneration: Buff {
    private readonly float _regenAmount;
    
    public Regeneration(float regenAmount, SimTime? duration) 
        : base(BuffType.Regeneration, duration) {
        _regenAmount = regenAmount;
    }
    
    public override void Update(Warlock target) {
        target.Health += _regenAmount;
        base.Update(target);
    }
}