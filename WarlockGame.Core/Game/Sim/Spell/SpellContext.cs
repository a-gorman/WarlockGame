using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellContext {
    public required Warlock Caster { get; set; }
    public required Simulation Simulation { get; init; }
    
    public EntityManager EntityManager => Simulation.EntityManager;
    public EffectManager EffectManager => Simulation.EffectManager;
}