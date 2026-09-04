using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell;

class WarlockSpell {
    public int Id { get; }
    public SpellDefinition Definition { get; }
    public int SlotLocation { get; set; }
    
    public GameTimer Cooldown { get; set; } = GameTimer.FromTicks(0);
    public bool OnCooldown => !Cooldown.IsExpired;

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
        var context = new SpellContext(caster, castLocation, _simulation);

        switch (Definition) {
            case DirectionalSpell directionalSpell:
                directionalSpell.Effects.ForEach(x => x.Invoke(context, caster.Position, castLocation));
                break;
            case LocationSpell locationSpell:
                if (locationSpell.CastRange.HasValue) {
                    // Cast the spell at it's max range if cast beyond max range
                    castLocation = caster.Position + (castLocation - caster.Position).WithMaxLength(locationSpell.CastRange.Value);
                }

                locationSpell.Effects.ForEach(x => x.Invoke(context, castLocation));
                locationSpell.SelfEffects.ForEach(x => x.Invoke(context));
                break;
            case SelfCastSpell selfCastSpell:
                selfCastSpell.Effects.ForEach(x => x.Invoke(context));
                selfCastSpell.CastLocationEffects.ForEach(x => x.Invoke(context, caster.Position));
                break;
        }
    }
}