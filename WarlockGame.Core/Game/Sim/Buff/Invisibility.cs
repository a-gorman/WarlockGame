using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buff;

class Invisibility: IBuff {
    public int Id { get; set; }
    public int TypeId { get; } = 1;
    public bool IsExpired { get; set; }
    public void Update(Warlock context) {
        throw new System.NotImplementedException();
    }
}