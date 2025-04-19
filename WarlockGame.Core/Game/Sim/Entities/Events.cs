namespace WarlockGame.Core.Game.Sim.Entities;

class OnDamagedEventArgs {
    public required Entity Damaged { get; init; }
    public required Entity DamageSource { get; init; }
    public required float Amount { get; init; }
}

struct OnCollisionEventArgs {
    public required Entity Source { get; init; }
    public required Entity Other { get; init; }
}