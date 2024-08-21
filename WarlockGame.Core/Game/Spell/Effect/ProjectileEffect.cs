using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Spell.Effect; 

class ProjectileEffect: IDirectionalSpellEffect {
    
    private int _speed = 5;
    private readonly Sprite _sprite;
    private readonly IReadOnlyList<ILocationSpellEffect> _effects;

    public ProjectileEffect(Sprite sprite, IEnumerable<ILocationSpellEffect> effects) {
        _sprite = sprite;
        _effects = effects.ToList();
    }

    public void Invoke(Warlock caster, Vector2 castLocation, Vector2 castDirection) {
        EntityManager.Add(new Projectile(
            position: castLocation, 
            velocity: castDirection.ToNormalized() * _speed,
            caster: caster,
            sprite: _sprite,
            effects: _effects));
    }
}