using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class AoeTargetsLocationComponent : IAoeTargetsSpellComponent {

    public List<ILocationSpellComponent> Components { get; init; } = [];
    public List<Func<AoeTargetInfo, ILocationSpellComponent>> DynamicComponents { get; init; } = [];
    public List<Action<SpellContext, IReadOnlyCollection<AoeTargetInfo>>> Actions { get; init; } = [];
    
    public void Invoke(SpellContext context, IReadOnlyCollection<AoeTargetInfo> targets) {
        foreach (var spellComponent in Components) {
            foreach (var targetInfo in targets) {
                spellComponent.Invoke(context, targetInfo.Entity.Position);
            }
        }
        
        foreach (var spellComponent in DynamicComponents) {
            foreach (var target in targets) {
                spellComponent.Invoke(target).Invoke(context, target.Entity.Position);
            }
        }
        
        foreach (var action in Actions) {
            action.Invoke(context, targets);
        }
    }
}