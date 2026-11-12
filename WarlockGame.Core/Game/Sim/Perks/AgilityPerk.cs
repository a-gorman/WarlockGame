using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Buffs.Components;

namespace WarlockGame.Core.Game.Sim.Perks;

class AgilityPerk: PermanentBuffPerk {
    private const float MoveSpeedFactor = 1.25f;
    private const float TurnSpeedFactor = 1.6f;

    public AgilityPerk() : base(
        10,
        "Agility",
        $"Permanently enhances your move speed by {MoveSpeedFactor - 1:0.##%} and your turn speed by {TurnSpeedFactor - 1:0.##%}",
        Art.AgilityIcon) { }

    protected override Buff CreateBuff(Simulation sim) {
        return new Buff(Buff.BuffType.Agility, duration: null,
            [new SpeedBuffComponent(MoveSpeedFactor, TurnSpeedFactor)]);
    }
}