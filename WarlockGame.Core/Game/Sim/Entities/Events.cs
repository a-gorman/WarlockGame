namespace WarlockGame.Core.Game.Sim.Entities;

class OnDamagedEventArgs {
    public required Entity Source { get; init; }
    public required Entity? DamageSource { get; init; }
    public required float Amount { get; init; }
    public required DamageType DamageTypes { get; init; }
}

struct OnCollisionEventArgs {
    public required Entity Source { get; init; }
    public required Entity Other { get; init; }
}

struct OnPushedEventArgs {
    public required Entity Source { get; init; }
    public required Vector2 Force { get; init; }
}