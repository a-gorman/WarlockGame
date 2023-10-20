namespace NeonShooter.Core.Game.Entity.Projectile;

interface IProjectile: IEntity
{
    void OnHit();
}