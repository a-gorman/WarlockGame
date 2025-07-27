using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.Component; 

class ProjectileComponent: IDirectionalSpellComponent {
    
    private int _speed = 8;
    private readonly Sprite _sprite;
    private readonly Func<Behavior[]>? _behaviors;
    private readonly bool _ignoreCaster;
    private readonly IReadOnlyList<ILocationSpellComponent> _effects;

    public ProjectileComponent(Sprite sprite, 
        IEnumerable<ILocationSpellComponent> effects, 
        Func<Behavior[]>? behaviors = null,
        bool ignoreCaster = true) {
        _sprite = sprite;
        _behaviors = behaviors;
        _ignoreCaster = ignoreCaster;
        _effects = effects.ToList();
    }

    public void Invoke(SpellContext context, Vector2 castLocation, Vector2 invokeDirection) {
        context.EntityManager.Add(new Projectile(
            position: castLocation, 
            velocity: invokeDirection.ToNormalized() * _speed,
            context: context,
            sprite: _sprite,
            effects: _effects,
            ignoreCaster: _ignoreCaster)
            .Also(x => _behaviors?.Invoke().ForEach(b => x.AddBehaviors(b))));
    }
}