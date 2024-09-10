using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.Component;

class SelfCastPositionComponent : ISelfSpellComponent {
    
    public required ILocationSpellComponent Component { get; init; }
    
    public void Invoke(Warlock caster) {
        Component.Invoke(caster, caster.Position);
    }
}