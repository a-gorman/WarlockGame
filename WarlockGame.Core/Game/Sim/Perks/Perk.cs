using System.Collections.Generic;

namespace WarlockGame.Core.Game.Sim.Perks;

class Perk {
    public Perk(PerkType type) {
        Type = type;
    }
    public int Id { get; set; }
    public PerkType Type { get; set; }
    public virtual void Update(Simulation sim) { }
    public virtual void OnChosen(int forceId, Simulation sim) { }
    public virtual void OnPerkRemoved(int forceId, Simulation sim) { }
}