using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Spell.Component; 

class ProjectileComponent: IDirectionalSpellComponent {
    
    private int _speed = 5;
    private readonly Sprite _sprite;
    private readonly IReadOnlyList<ILocationSpellComponent> _effects;

    public ProjectileComponent(Sprite sprite, IEnumerable<ILocationSpellComponent> effects) {
        _sprite = sprite;
        _effects = effects.ToList();
    }

    public void Invoke(Warlock caster, Vector2 castLocation, Vector2 invokeDirection) {
        EntityManager.Add(new Projectile(
            position: castLocation, 
            velocity: invokeDirection.ToNormalized() * _speed,
            caster: caster,
            sprite: _sprite,
            effects: _effects));
    }
}