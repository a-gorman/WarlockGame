using System.Collections.Generic;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class SelfCastPositionComponent : ISelfSpellComponent {
    
    public required List<ILocationSpellComponent> Components { get; init; }
    
    public void Invoke(SpellContext context) {
        foreach (var component in Components) {
            component.Invoke(context, context.Caster.Position);
        }
    }
}