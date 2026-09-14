using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class RedirectProjectileComponent: IAoeTargetsSpellComponent {
    private readonly Vector2 _direction;
    
    public RedirectProjectileComponent(Vector2 direction) {
        _direction = direction;
    }

    public void Invoke(SpellContext context, IReadOnlyCollection<AoeTargetInfo> targets) {
        foreach (var target in targets) {
            target.Entity.Velocity = _direction.WithLength(target.Entity.Velocity.Length());
        }    
    }
}