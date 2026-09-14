using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class SwapComponent: IAoeTargetsSpellComponent {
    public void Invoke(SpellContext context, IReadOnlyCollection<AoeTargetInfo> targets) {
        if(targets.Count == 0) return;

        var casterPos = context.Caster.Position;
        context.Caster.Position = targets.First().Entity.Position - targets.First().OriginTargetDisplacement;
        foreach(var target in targets) {
            target.Entity.Position = casterPos + target.OriginTargetDisplacement;
        }
    }
}