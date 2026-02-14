using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.Component;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell;

class WarlockSpell {
    public int Id { get; }
    public SpellDefinition Definition { get; }
    public int SlotLocation { get; set; }
    
    public GameTimer Cooldown { get; set; } = GameTimer.FromTicks(0);
    public bool OnCooldown => !Cooldown.IsExpired;

    public OneOf<IDirectionalSpellComponent[], ILocationSpellComponent[], ISelfSpellComponent[]> Effect => Definition.Effects;

    private readonly Simulation _simulation;
    
    public WarlockSpell(int id, SpellDefinition definition, Simulation simulation) {
        Id = id;
        _simulation = simulation;
        Definition = definition;
    }

    public void Update() {
        Cooldown = Cooldown.Decrement();
    }

    public void DoCast(Warlock caster, Vector2 castTarget) {
        Cooldown = Definition.CooldownTime.ToTimer();
        var context = new SpellContext
        {
            Caster = caster,
            Simulation = _simulation
        };
        Definition.Effects.Switch(
            directionalEffect => directionalEffect.ForEach(x => x.Invoke(context, caster.Position, castTarget)),
            locationEffect => {
                var castLocation = castTarget;
                if(Definition.CastRange.HasValue) {
                    // Cast the spell at it's max range (or shorter)
                    castLocation = caster.Position + (castLocation - caster.Position).WithMaxLength(Definition.CastRange.Value);
                }
                locationEffect.ForEach(x => x.Invoke(context, castLocation));
            },
            selfEffect => selfEffect.ForEach(x => x.Invoke(context))
        );
    }
}