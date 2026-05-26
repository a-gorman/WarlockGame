using System.Collections.Generic;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

struct AoeResult {
    public required List<TargetInfo> Targets { get; init; }
    public required Vector2 Center { get; init; }
    public required float SoundRadius { get; init; }
}