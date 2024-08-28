using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.AreaOfEffect;

class TargetInfo {
    public required IEntity Entity { get; init; }
    public required Vector2 Displacement { get; init; }
    public required float FalloffFactor { get; init; }
}