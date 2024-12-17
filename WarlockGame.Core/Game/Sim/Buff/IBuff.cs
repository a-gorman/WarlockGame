using WarlockGame.Core.Game.Sim.Entity;

namespace WarlockGame.Core.Game.Sim.Buff;

interface IBuff {
    bool IsExpired { get; set; }

    public void Update(Warlock target);
}