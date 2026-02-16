using System.Collections.Generic;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DirectionalComponent : IDirectionalSpellComponent {

    public List<ILocationSpellComponent> Components { get; init; } = [];

    public void Invoke(SpellContext context, Vector2 castLocation, Vector2 invokeDirection) {
        foreach (var component in Components) {
            component.Invoke(context, invokeDirection);
        }
    }
}