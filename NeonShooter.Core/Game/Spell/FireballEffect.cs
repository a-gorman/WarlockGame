using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Entity.Projectile;

namespace NeonShooter.Core.Game.Spell; 

public class FireballEffect: ICastEffect {
    
    private int _speed = 5;

    public void OnCast(Vector2 castPosition, Vector2 castDirection) {
        EntityManager.Add(new FireballProjectile(castPosition, castDirection * _speed));
    }
}