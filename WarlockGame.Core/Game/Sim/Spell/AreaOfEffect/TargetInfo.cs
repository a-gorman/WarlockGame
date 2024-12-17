using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Entity;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

class TargetInfo {
    public required IEntity Entity { get; init; }
    public required Vector2 DisplacementAxis1 { get; init; }
    public required Vector2 DisplacementAxis2 { get; init; }
    public required float FalloffFactor { get; init; }
}