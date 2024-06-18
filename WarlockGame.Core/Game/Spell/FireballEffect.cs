using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Entity.Projectile;

namespace WarlockGame.Core.Game.Spell; 

public class FireballEffect: ICastEffect {
    
    private int _speed = 5;

    public void OnCast(IEntity caster, Vector2 castDirection) {
        EntityManager.Add(new FireballProjectile(caster.Position, castDirection * _speed, caster));
    }
}