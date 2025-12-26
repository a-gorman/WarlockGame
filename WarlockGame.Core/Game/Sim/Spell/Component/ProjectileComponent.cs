using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.Component; 

class ProjectileComponent: IDirectionalSpellComponent {
    
    private readonly int _speed;
    private readonly float _radius;
    private readonly Sprite _sprite;
    private readonly Func<Behavior[]>? _behaviors;
    private readonly ILocationSpellComponent[] _effects;

    public ProjectileComponent(Sprite sprite, 
        IEnumerable<ILocationSpellComponent> effects, 
        Func<Behavior[]>? behaviors = null,
        int speed = 8,
        float radius = 8) {
        _sprite = sprite;
        _behaviors = behaviors;
        _speed = speed;
        _radius = radius;
        _effects = effects.ToArray();
    }

    public void Invoke(SpellContext context, Vector2 castLocation, Vector2 invokeDirection) {
        context.EntityManager.Add(new Projectile(
            position: castLocation, 
            velocity: invokeDirection.ToNormalized() * _speed,
            radius: _radius,
            context: context,
            sprite: _sprite,
            effects: _effects)
            .Also(x => _behaviors?.Invoke().ForEach(b => x.AddBehaviors(b))));
    }
}