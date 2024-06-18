using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Entity.Projectile;

namespace WarlockGame.Core.Game.Spell; 

class Explosion : IProjectileEffect {
  
    public required float Radius { get; init; }
    
    public required float Damage { get; init; }
    
    public required int Force { get; init; }
    
    public required float Falloff { get; init; }

    public void OnImpact(IProjectile source) {
        foreach (var entity in EntityManager.GetNearbyEntities(source.Position, Radius))
        {
            switch (entity)
            {
                case Warlock player:
                    var falloffFactor = Radius / (player.Position - source.Position).Length();
                    player.Push( (int)(Force * falloffFactor), player.Position - source.Position);
                    player.Damage(Damage * falloffFactor, source);
                    break;
            }
        }
    }
}