using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class DeflectComponent: BuffComponent {
    private readonly Simulation _sim;
    
    public DeflectComponent(Simulation sim)
    {
        _sim = sim;
    }

    public override void Update(Buff buff, Warlock target)
    {
        var entities = _sim.EntityManager.GetNearbyEntities(target.Position, 400);
        foreach (var entity in entities)
        {
            entity.Velocity *= -1;
        }
    }
}