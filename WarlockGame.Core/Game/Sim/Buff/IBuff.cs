using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.Sim.Buff;

interface IBuff {
    bool IsExpired { get; set; }

    public void Update(Warlock context);
}