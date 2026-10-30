using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class CastWhenDamaged: BuffComponent {
    private readonly ISelfSpellComponent[] _castEffects;
    private readonly Simulation _sim;
    private readonly SimTime _cooldownDuration;
    private GameTimer _cooldown = GameTimer.Expired();

    public CastWhenDamaged(Simulation sim, SimTime cooldown, ISelfSpellComponent[] castEffects) {
        _castEffects = castEffects;
        _sim = sim;
        _cooldownDuration = cooldown;
    }

    public override void Update(Buff buff, Warlock target) {
        _cooldown = _cooldown.Decremented();
    }

    public override void OnAdd(Buff buff, Warlock target) {
        target.OnDamaged += HandledTargetDamaged;
    }

    public override void OnRemove(Buff buff, Warlock target) {
        target.OnDamaged -= HandledTargetDamaged;
    }

    private void HandledTargetDamaged(OnDamagedEventArgs args) {
        if (!_cooldown.IsExpired) {
            return;
        }
        
        if (args.DamagedEntity is Warlock caster) {
            var context = new SpellContext(caster, Vector2.Zero, _sim);
            foreach (var component in _castEffects) {
                component.Invoke(context);
            }

            _cooldown = _cooldownDuration.ToTimer();
        } else {
            Logger.Warning("Non-warlock tried casting a spell via CastWhenDamaged buff component", Logger.LogType.Simulation);
        }
    }
}
