using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.Effect; 

class Explosion : ILocationSpellEffect {
  
    public required float Radius { get; init; }
    
    public required float Damage { get; init; }
    
    public required int Force { get; init; }

    public bool IgnoreCaster { get; init; } = false;
    
    public required float Falloff { get; init; }

    public void Invoke(Warlock caster, Vector2 invokeLocation) {
        foreach (var entity in EntityManager.GetNearbyEntities(invokeLocation, Radius))
        {
            switch (entity)
            {
                case Warlock player:
                    if(IgnoreCaster && player == caster) { continue; }
                    
                    var falloffFactor = Radius / (player.Position - invokeLocation).Length();
                    player.Push( (int)(Force * falloffFactor), player.Position - invokeLocation);
                    player.Damage(Damage * falloffFactor, caster);
                    break;
            }
        }
    }
}