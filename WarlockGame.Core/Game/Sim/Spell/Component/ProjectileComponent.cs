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
    private readonly SimTime? _maxLife = null;

    public ProjectileComponent(Sprite sprite, 
        IEnumerable<ILocationSpellComponent> effects, 
        Func<Behavior[]>? behaviors = null,
        int speed = 8,
        float radius = 8,
        float? maxRange = null) {
        _sprite = sprite;
        _behaviors = behaviors;
        _speed = speed;
        _radius = radius;
        _effects = effects.ToArray();
        if (maxRange != null) {
            _maxLife = SimTime.FromTicks(maxRange / speed);
        }
    }

    public void Invoke(SpellContext context, Vector2 invokeLocation, Vector2 invokeDirection) {
        val projectile = new Projectile(
            position: invokeLocation, 
            velocity: invokeDirection.ToNormalized() * _speed,
            radius: _radius,
            context: context,
            sprite: _sprite,
            effects: _effects);
        
        if (_maxLife != null) {
            projectile.AddBehaviors(new TimedLife<Projectile>(_maxLife, x => x.TriggerEffects()));
        }

        if (_behaviors != null) {
            projectile.AddBehaviors(_behaviors);
        }

        context.EntityManager.Add(projectile);
    }
}