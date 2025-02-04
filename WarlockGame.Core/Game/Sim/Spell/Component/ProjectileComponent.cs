using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Entity;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.Component; 

class ProjectileComponent: IDirectionalSpellComponent {
    
    private int _speed = 8;
    private readonly Sprite _sprite;
    private readonly IReadOnlyList<ILocationSpellComponent> _effects;

    public ProjectileComponent(Sprite sprite, IEnumerable<ILocationSpellComponent> effects) {
        _sprite = sprite;
        _effects = effects.ToList();
    }

    public void Invoke(SpellContext context, Vector2 castLocation, Vector2 invokeDirection) {
        context.EntityManager.Add(new Projectile(
            position: castLocation, 
            velocity: invokeDirection.ToNormalized() * _speed,
            context: context,
            sprite: _sprite,
            effects: _effects));
    }
}