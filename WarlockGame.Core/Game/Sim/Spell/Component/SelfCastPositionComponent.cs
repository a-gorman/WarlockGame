using WarlockGame.Core.Game.Sim.Entity;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class SelfCastPositionComponent : ISelfSpellComponent {
    
    public required ILocationSpellComponent Component { get; init; }
    
    public void Invoke(Warlock caster) {
        Component.Invoke(caster, caster.Position);
    }
}