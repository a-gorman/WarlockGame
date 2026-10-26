using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class SwapComponent: IAoeTargetsSpellComponent {
    public void Invoke(SpellContext context, Vector2 origin, IReadOnlyCollection<AoeTargetInfo> targets) {
        if (targets.Count == 0) {
            context.Caster.Position = origin;
            return;
        }

        var startingCasterPos = context.Caster.Position;
        var averageDisplacement = Vector2.Zero;
        foreach(var target in targets) {
            averageDisplacement += target.OriginTargetDisplacement;
            target.Entity.Position = startingCasterPos + target.OriginTargetDisplacement;
        }

        context.Caster.Position = averageDisplacement / targets.Count + origin;
    }
}