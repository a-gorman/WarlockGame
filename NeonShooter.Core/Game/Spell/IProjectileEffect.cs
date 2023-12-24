using NeonShooter.Core.Game.Entity.Projectile;

namespace NeonShooter.Core.Game.Spell; 

interface IProjectileEffect {
    void OnImpact(IProjectile source);
}