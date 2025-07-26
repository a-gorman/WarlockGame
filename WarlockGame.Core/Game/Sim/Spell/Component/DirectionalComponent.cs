using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

class DirectionalComponent : IDirectionalSpellComponent {

    public List<ILocationSpellComponent> Components { get; init; } = [];
    public List<Func<TargetInfo, ILocationSpellComponent>> DynamicComponents { get; init; } = [];
    public List<Action<SpellContext, IReadOnlyCollection<TargetInfo>>> Actions { get; init; } = [];
    
    public void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets) {
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

    public void Invoke(SpellContext context, Vector2 castLocation, Vector2 invokeDirection) {
        
        
    }
}