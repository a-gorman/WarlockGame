using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Effect.Display;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class SpriteComponent : BuffComponent {
    private readonly Simulation _sim;

    private readonly Sprite _sprite;
    private int _transformationId;
    private Vector2 _spriteOffset;
    
    private SpriteEffect _effect = null!;
    
    public SpriteComponent(Sprite sprite, Vector2 spriteOffset, Simulation sim) {
        _sprite = sprite;
        _sim = sim;
        _spriteOffset = spriteOffset;
    }

    public override void OnAdd(Buff buff, Warlock target) {
        _spriteOffset = -_spriteOffset.Rotated(target.Orientation);
        
        _effect = new SpriteEffect(_sprite, target.Position + _spriteOffset, duration: null, orientation: target.Orientation);
        _sim.EffectManager.Add(_effect);
    }

    public override void OnRemove(Buff buff, Warlock target) {
        _effect.IsExpired = true;
    }
    
    public override void Update(Buff buff, Warlock target) {
        _effect.Position = target.Position + _spriteOffset;
    }
}