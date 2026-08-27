using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Effect.Display;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Buffs;

class SparkJumpBuff : Buff {
    public float Height { get; private set; }
    public float VerticalVelocity { get; private set; }
    
    private readonly Vector2 _displacementPerTick;
    private readonly float _heightScaleFactor = 0.2f;
    private readonly float _acceleration;
    private readonly Simulation _sim;
    private int _transformationId;
    private Vector2 _spriteOffset;
    
    private SpriteEffect _travelSprite = null!;
    
    public SparkJumpBuff(Simulation sim, Vector2 displacement, float height, SimTime duration) : base(BuffType.Jumping, duration) {
        _sim = sim;
        _displacementPerTick = displacement / duration.Ticks;
        
        _acceleration = - 8f * height / duration.Ticks.Squared();
        VerticalVelocity =  _acceleration * -0.5f * duration.Ticks;
        Stacking = StackingType.None;
    }

    public override void OnAdd(Warlock target) {
        _spriteOffset = -_displacementPerTick.ToNormalized() * 100;
        
        var sprite = Sprite.FromGridSpriteSheet(Art.SparkJumpTravel, 3, 3, SimTime.OfSeconds(0.05f), scale: 2.3f);
        _travelSprite = new SpriteEffect(sprite, target.Position + _spriteOffset, duration: null, orientation: (_displacementPerTick).ToAngle());
        _sim.EffectManager.Add(_travelSprite);
        
        _transformationId = target.Sprite.AddTransformation(1f);
        target.Jumping = true;
    }

    public override void OnRemove(Warlock target) {
        if (_transformationId != 0) {
            target.Sprite.RemoveTransformation(_transformationId);
        }

        _travelSprite.IsExpired = true;
        target.Jumping = false;
    }
    
    protected override void OnUpdate(Warlock target) {
        Height += VerticalVelocity + _acceleration/2;
        VerticalVelocity += _acceleration;
        target.Sprite.ChangeTransformation(_transformationId, (Height + 1) * _heightScaleFactor);
        target.Position += _displacementPerTick;

        _travelSprite.Position = target.Position + _spriteOffset;
    }
}