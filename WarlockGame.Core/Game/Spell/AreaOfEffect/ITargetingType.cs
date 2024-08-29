using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

interface IDirectionalShape {
    public List<TargetInfo> GatherTargets(Warlock caster, Vector2 castLocation, Vector2 invokeDirection);
    public Texture2D? Texture { get; }
}

interface ILocationShape {
    public List<TargetInfo> GatherTargets(Warlock caster, Vector2 invokeLocation);
    public Texture2D? Texture { get; }
}