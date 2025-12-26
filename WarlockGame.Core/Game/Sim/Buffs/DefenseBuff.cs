using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Buffs;

class DefenseBuff : Buff {
    public float GenericDefenseModifier { get; init; } = 1;
    public float PlayerDefenseModifier { get; init; } = 1;
    public float EnvironmentDefenseModifier { get; init; } = 1;
    public float BoundsDefenseModifier { get; init; } = 1;

    public DefenseBuff(SimTime? duration) : base(BuffType.Defense, duration) { }

    public override void OnAdd(Warlock target) {
        target.GenericDefense *= GenericDefenseModifier;
        target.PlayerDefense *= PlayerDefenseModifier;
        target.EnvironmentDefense *= EnvironmentDefenseModifier;
        target.BoundsDefense *= BoundsDefenseModifier;
    }

    public override void OnRemove(Warlock target) {
        target.GenericDefense /= GenericDefenseModifier;
        target.PlayerDefense /= PlayerDefenseModifier;
        target.EnvironmentDefense /= EnvironmentDefenseModifier;
        target.BoundsDefense /= BoundsDefenseModifier;
    }
}