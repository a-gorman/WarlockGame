using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

class CircleTarget : ILocationShape {
    public bool IgnoreCaster { get; init; } = false;
    public required float Radius { get; init; }
    public Texture2D? Texture { get; init; }
    public Falloff.FalloffFactor FalloffFactor { get; init; } = Falloff.Linear;
    
    public List<TargetInfo> GatherTargets(Warlock caster, Vector2 origin) {
        // Texture?.Run(x => EffectManager.Add(new CircleEffect());
        
        return EntityManager.GetNearbyEntities(origin, Radius)
                            .Where(x => !IgnoreCaster || x != caster)
                            .Select(x => new TargetInfo
                            {
                                Entity = x,
                                DisplacementAxis1 = x.Position - origin,
                                DisplacementAxis2 = x.Position - origin,
                                FalloffFactor = FalloffFactor.Invoke(x.Position - origin, Radius, x.Radius)
                            })
                            .ToList();
    }

}

