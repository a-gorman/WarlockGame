using WarlockGame.Core.Game.Sim.Effect;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Spell;

class SpellContext {
    public Warlock Caster { get; set; }
    public Vector2 TargetPosition { get; set; }
    public Simulation Simulation { get; init; }
    
    public Vector2 CastFromPosition { get; set; }
    

    public SpellContext(Warlock caster, Vector2 targetPosition, Simulation simulation) {
        Caster = caster;
        TargetPosition = targetPosition;
        Simulation = simulation;

        CastFromPosition = caster.Position;
    }
    
    public EntityManager EntityManager => Simulation.EntityManager;
    public EffectManager EffectManager => Simulation.EffectManager;
}