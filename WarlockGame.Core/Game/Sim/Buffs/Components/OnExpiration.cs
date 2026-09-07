

namespace WarlockGame.Core.Game.Sim.Buffs.Behaviors;

class OnExpiration: BuffComponent {

    private readonly SpellContext _context;
    private readonly ISelfSpellComponent[] _components;

    public OnExpiration(SpellContext spellContext, ISelfSpellComponent[] components) {
        _context = spellContext;
    }

    public override void OnExpiration(Buff buff, Warlock target) {
        foreach(var component in _components) {
            component.invoke(component)
        }
    }
}