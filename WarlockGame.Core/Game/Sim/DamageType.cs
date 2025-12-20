using System;

namespace WarlockGame.Core.Game.Sim;

[Flags]
enum DamageType {
    None = 0,
    Player = 1,
    Environment = 1<<1,
    Bounds = 1<<2,
}