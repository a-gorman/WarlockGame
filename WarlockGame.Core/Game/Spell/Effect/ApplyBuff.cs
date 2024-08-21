using System;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Buff;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game.Spell.Effect;

class ApplyBuffArea : ILocationSpellEffect {
    private readonly int _radius;
    private readonly Func<Warlock,IBuff> _buffConstructor;

    public bool IgnoreCaster { get; init; } = false;
    
    public ApplyBuffArea(int radius, Func<Warlock,IBuff> buffConstructor) {
        _radius = radius;
        _buffConstructor = buffConstructor;
    }

    public void Invoke(Warlock caster, Vector2 invokeLocation) {
        foreach (var entity in EntityManager.GetNearbyEntities(invokeLocation, _radius)) {
            switch (entity)
            {
                case Warlock player:
                    if(IgnoreCaster && player == caster) { continue; }
                    
                    player.AddBuff(_buffConstructor.Invoke(caster));
                    break;
            }
        }
    }

}