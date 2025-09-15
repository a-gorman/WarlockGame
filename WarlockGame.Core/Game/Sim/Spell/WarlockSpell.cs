using Microsoft.Xna.Framework.Graphics;
using OneOf;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell;

class WarlockSpell {
    public required int Id { get; set; }
    public required string Name { get; init; }
    public required int SpellTypeId { get; init; }
    public int SlotLocation { get; set; }
    public required SimTime CooldownTime { get; init; }
    public required Texture2D SpellIcon { get; init; }
    public required OneOf<IDirectionalSpellComponent, ILocationSpellComponent, ISelfSpellComponent> Effect { get; init; }
    public GameTimer Cooldown { get; set; } = GameTimer.FromTicks(0);
    public bool OnCooldown => !Cooldown.IsExpired;

    private readonly Simulation _simulation;
    
    public WarlockSpell(Simulation simulation) {
        _simulation = simulation;
    }

    public void Update() {
        Cooldown = Cooldown.Decrement();
    }

    public void DoCast(Warlock caster, Vector2? direction) {
        Cooldown = CooldownTime.ToTimer();
        var context = new SpellContext
        {
            Caster = caster,
            Simulation = _simulation
        };
        Effect.Switch(
            directionalEffect => directionalEffect.Invoke(context, caster.Position, direction ?? Vector2.Zero),
            locationEffect => locationEffect.Invoke(context, direction ?? Vector2.Zero),
            selfEffect => selfEffect.Invoke(context)
        );
    }
}