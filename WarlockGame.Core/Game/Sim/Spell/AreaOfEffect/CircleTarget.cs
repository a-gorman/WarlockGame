using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

class CircleTarget : ILocationShape {
    public bool IgnoreCaster { get; init; } = false;
    public required float Radius { get; init; }
    public Texture2D? Texture { get; init; }
    public Falloff.FalloffFactor FalloffFactor { get; init; } = Falloff.Linear;
    
    public List<TargetInfo> GatherTargets(SpellContext context, Vector2 origin) {
        // Texture?.Run(x => EffectManager.Add(new CircleEffect());
        
        return context.EntityManager.GetNearbyEntities(origin, Radius)
                            .Where(x => !IgnoreCaster || x != context.Caster)
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

