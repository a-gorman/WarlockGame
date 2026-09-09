using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Effect.Display;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Buffs;

class SpriteComponent : BuffComponent {
    private readonly Simulation _sim;

    private readonly Sprite _sprite;
    private int _transformationId;
    private Vector2 _spriteOffset;
    
    private SpriteEffect _travelSprite = null!;
    
    public SpriteComponent(Sprite sprite, Vector2 spriteOffset, Simulation sim) {
        _sim = sim;
        _spriteOffset = spriteOffset;
    }

    public override void OnAdd(Warlock target) {
        _spriteOffset = -_spriteOffset.Rotated(orientation);
        
        _travelSprite = new SpriteEffect(_sprite, target.Position + _spriteOffset, duration: null, orientation: (_displacementPerTick).ToAngle());
        _sim.EffectManager.Add(_travelSprite);
    }

    public override void OnRemove(Warlock target) {
        _travelSprite.IsExpired = true;
    }
    
    protected override void OnUpdate(Warlock target) {
        Height += VerticalVelocity + _acceleration/2;
        VerticalVelocity += _acceleration;

        _travelSprite.Position = target.Position + _spriteOffset;
    }
}