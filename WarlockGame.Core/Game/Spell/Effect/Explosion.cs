using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell; 

class Explosion : ILocationSpellEffect {
  
    public required float Radius { get; init; }
    
    public required float Damage { get; init; }
    
    public required int Force { get; init; }

    public bool IgnoreCaster { get; init; } = false;
    
    public required float Falloff { get; init; }

    public void Invoke(IEntity caster, Vector2 castLocation) {
        foreach (var entity in EntityManager.GetNearbyEntities(castLocation, Radius))
        {
            switch (entity)
            {
                case Warlock player:
                    if(IgnoreCaster && player == caster) { continue; }
                    
                    var falloffFactor = Radius / (player.Position - castLocation).Length();
                    player.Push( (int)(Force * falloffFactor), player.Position - castLocation);
                    player.Damage(Damage * falloffFactor, caster);
                    break;
            }
        }
    }
}