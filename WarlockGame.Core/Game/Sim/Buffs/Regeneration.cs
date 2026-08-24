using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class Regeneration: Buff {
    private readonly float _regenAmount;
    
    public Regeneration(float regenAmount, SimTime? duration) 
        : base(BuffType.Regeneration, duration) {
        _regenAmount = regenAmount;
    }
    
    protected override void OnUpdate(Warlock target) {
        target.Health += _regenAmount;
    }
}