using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

interface IDirectionalShape {
    public List<TargetInfo> GatherTargets(SpellContext context, Vector2 castLocation, Vector2 invokeDirection);
    public Texture2D? Texture { get; }
}

interface ILocationShape {
    public List<TargetInfo> GatherTargets(SpellContext context, Vector2 invokeLocation);
    public Texture2D? Texture { get; }
}