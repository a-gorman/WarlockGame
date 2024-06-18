using WarlockGame.Core.Game.Entity.Projectile;

namespace WarlockGame.Core.Game.Spell; 

interface IProjectileEffect {
    void OnImpact(IProjectile source);
}