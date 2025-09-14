using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buff;

interface IBuff {
    int Id { get; set; }
    int TypeId { get; }
    bool IsExpired { get; set; }

    public void Update(Warlock context);
}