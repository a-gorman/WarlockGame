using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

delegate bool AoeSelector(Entity entity, SpellContext context);

static class AoeSelectors {
    public static AoeSelector All { get; } = (_, _) => true;
    public static AoeSelector Warlocks { get; } = (x, context) => x is Warlock;
    public static AoeSelector WarlocksIgnoreCaster { get; } = (x, context) => x != context.Caster && x is Warlock;
    public static AoeSelector AllIgnoreCaster { get; } = (x, context) => x != context.Caster;
    public static AoeSelector Projectiles { get; } = (x, context) => x is Projectile;
}