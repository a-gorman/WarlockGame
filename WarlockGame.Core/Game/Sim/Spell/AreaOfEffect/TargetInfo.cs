using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

class TargetInfo {
    public required Entity Entity { get; init; }
    public required Vector2 OriginTargetDisplacement { get; init; }
    public required Vector2 DisplacementAxis2 { get; init; }
    public required float FalloffFactor { get; init; }
}