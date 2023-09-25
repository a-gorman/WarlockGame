using NeonShooter.Core.Game.Entity;

namespace NeonShooter.Core.Game.Projectile;

interface IProjectile: IEntity
{
    void OnHit();
}