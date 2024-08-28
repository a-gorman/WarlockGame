using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

class CircleTarget : ILocationShape {
    public bool IgnoreCaster { get; init; } = false;
    public required float Radius { get; init; }

    public List<TargetInfo> GatherTargets(Warlock caster, Vector2 origin) {
        return EntityManager.GetNearbyEntities(origin, Radius)
                            .Where(x => !IgnoreCaster || x != caster)
                            .Select(x => new TargetInfo
                            {
                                Entity = x,
                                Displacement = x.Position - origin,
                                FalloffFactor = 1 - float.Clamp((x.Position - origin).Length() - x.Radius / Radius, 0, 1)
                            })
                            .ToList();
    }
}

