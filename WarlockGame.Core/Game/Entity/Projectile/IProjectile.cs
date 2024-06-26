namespace WarlockGame.Core.Game.Entity.Projectile;

interface IProjectile: IEntity
{
    void OnHit();
    
    IEntity Parent { get; }
}