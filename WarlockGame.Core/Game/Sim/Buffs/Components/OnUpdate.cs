
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class OnUpdate: BuffComponent {
    private GameTimer _timer;

    private readonly SimTime _repetitionRate;
    private readonly SpellContext _context;
    private readonly ISelfSpellComponent[] _components;

    public OnUpdate(SpellContext context, SimTime repeatEvery, ISelfSpellComponent[] components, bool triggerOnApplication = false) {
        _context = context;
        _components = components;
        _repetitionRate = repeatEvery;
        _timer = triggerOnApplication ? GameTimer.FromTicks(0) : repeatEvery.ToTimer();
    }

    public override void Update(Buff buff, Warlock target) {
        _timer = _timer.Decremented();
        if (!_timer.IsExpired) {
            return;
        }

        foreach (var component in _components) {
            component.Invoke(_context);
        }

        _timer = _repetitionRate.ToTimer();
    }
}