using System;

namespace WarlockGame.Core.Game.Sim;

[Flags]
enum DamageType {
    None = 0,
    Player = 1,
    Environment = 1<<1,
    Bounds = 1<<2,
    Shared = 1<<3,
    Reflected = 1<<3,
}

static class DamageTypeExtensions {
    extension(DamageType sourceType) {
        public bool HasType(DamageType damageType) => (damageType & sourceType) != 0;
    }
}