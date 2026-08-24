using System;
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

    public OneOf<IDirectionalSpellComponent[], ILocationSpellComponent[], ISelfSpellComponent[], Action<SpellContext, Vector2>[]> Effect => Definition.Effects;

    private readonly Simulation _simulation;
    
    public WarlockSpell(int id, SpellDefinition definition, Simulation simulation) {
        Id = id;
        _simulation = simulation;
        Definition = definition;
    }

    public void Update() {
        Cooldown = Cooldown.Decremented();
    }

    public void DoCast(Warlock caster, Vector2 castTarget) {
        Cooldown = Definition.CooldownTime.ToTimer();
        Definition.CastSound?.Play(caster.Position);
        var castLocation = castTarget;
        if(Definition.CastRange.HasValue) {
            // Cast the spell at it's max range if cast beyond max range
            castLocation = caster.Position + (castLocation - caster.Position).WithMaxLength(Definition.CastRange.Value);
        }
        var context = new SpellContext(caster, castLocation, _simulation);
        Definition.Effects.Switch(
            directionalEffect => directionalEffect.ForEach(x => x.Invoke(context, caster.Position, castTarget)),
            locationEffect => {
                locationEffect.ForEach(x => x.Invoke(context, castLocation));
            },
            selfEffect => selfEffect.ForEach(x => x.Invoke(context)),
            lambda => {
                lambda.ForEach(x => x.Invoke(context, castLocation));
            });
    }
}