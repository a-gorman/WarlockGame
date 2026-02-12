using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DelayedLocationComponent : ILocationSpellComponent {
    private readonly SimTime _delay;
    private readonly ILocationSpellComponent[] _components;

    public DelayedLocationComponent(SimTime delay, params ILocationSpellComponent[] components) {
        _delay = delay;
        _components = components;
    }
    
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        context.EffectManager.AddDelayedEffect(() => _components.ForEach(x => x.Invoke(context, invokeLocation)), _delay);
    }
}