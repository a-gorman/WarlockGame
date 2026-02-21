using System;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DelayedDirectionalComponent : IDirectionalSpellComponent {
    private readonly SimTime _delay;
    private readonly IDirectionalSpellComponent[] _components;
    private readonly Func<Vector2, Vector2> _direction;

    public DelayedDirectionalComponent(SimTime delay, 
        IDirectionalSpellComponent[] components,
        Func<Vector2, Vector2>? direction = null
        ) {
        _delay = delay;
        _components = components;
        _direction = direction ?? (x => x);
    }
    
    public void Invoke(SpellContext context, Vector2 invokeLocation, Vector2 invokeDirection) {
        context.EffectManager.AddDelayedEffect(() => _components.ForEach(x => x.Invoke(context, invokeLocation, _direction.Invoke(invokeDirection))), _delay);
    }
}