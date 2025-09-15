using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buff;

interface IBuff {
    int Id { get; set; }
    BuffType Type { get; }
    bool IsExpired { get; set; }

    public void Update(Warlock target);

    public enum BuffType {
        Invalid = 0,
        Invisibility,
        DamageOverTime = 0,
    }
}