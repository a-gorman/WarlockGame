using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

interface IDirectionalShape {
    public List<TargetInfo> GatherTargets(Warlock caster, Vector2 castLocation, Vector2 invokeDirection);
}

interface ILocationShape {
    public List<TargetInfo> GatherTargets(Warlock caster, Vector2 invokeLocation);
}