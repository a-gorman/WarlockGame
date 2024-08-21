using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Buff;

interface IBuff {
    bool IsExpired { get; set; }

    public void Update(Warlock target);
}