using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Spell.Component;

/// <summary>
/// Creates a new entity and adds it to the entity manager
/// </summary>
class AoeEntityComponent: IAoeTargetsSpellComponent {
    private readonly Func<SpellContext, AoeTargetInfo, Entity[]> _entityConstructor;
    
    public AoeEntityComponent(Func<SpellContext, AoeTargetInfo, Entity[]> entityConstructor) {
        _entityConstructor = entityConstructor;
    }

    public void Invoke(SpellContext context, IReadOnlyCollection<AoeTargetInfo> targets) {
        targets
            .SelectMany(x => _entityConstructor.Invoke(context, x))
            .ForEach(x => context.EntityManager.Add(x));
    }
}