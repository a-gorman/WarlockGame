using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Spell;

class WarlockSpell {
    public int Id { get; }
    public SpellDefinition Definition { get; }
    public int SlotLocation { get; set; }
    
    public GameTimer Cooldown { get; set; } = GameTimer.FromTicks(0);
    public bool OnCooldown => !Cooldown.IsExpired;

    public OneOf<IDirectionalSpellComponent, ILocationSpellComponent, ISelfSpellComponent> Effect => Definition.Effect;

    private readonly Simulation _simulation;
    
    public WarlockSpell(int id, SpellDefinition definition, Simulation simulation) {
        Id = id;
        _simulation = simulation;
        Definition = definition;
    }

    public void Update() {
        Cooldown = Cooldown.Decrement();
    }

    public void DoCast(Warlock caster, Vector2? direction) {
        Cooldown = Definition.CooldownTime.ToTimer();
        var context = new SpellContext
        {
            Caster = caster,
            Simulation = _simulation
        };
        Definition.Effect.Switch(
            directionalEffect => directionalEffect.Invoke(context, caster.Position, direction ?? Vector2.Zero),
            locationEffect => locationEffect.Invoke(context, direction ?? Vector2.Zero),
            selfEffect => selfEffect.Invoke(context)
        );
    }
}