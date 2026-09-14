using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.Sim.Spell.Component;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class OnExpiration: BuffComponent {

    private readonly SpellContext _context;
    private readonly IEntitySpellComponent[] _components;

    public OnExpiration(SpellContext spellContext, IEntitySpellComponent[] components) {
        _context = spellContext;
        _components = components;
    }

    public override void OnRemove(Buff buff, Warlock target) {
        if (buff.Timer.HasValue && buff.Timer.Value.IsExpired) {
            foreach (var component in _components) {
                component.Invoke(_context, target);
            }
        }
    }
}