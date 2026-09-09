using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Effect.Display;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class JumpComponent : BuffComponent {
    public float Height { get; private set; }
    public float VerticalVelocity { get; private set; }

    private readonly float _maxHeight;
    private readonly Simulation _sim;

    private Vector2 _displacementPerTick;
    private float _verticalAcceleration;
    private int _transformationId;
    
    public SparkJumpBuff(Simulation sim, Vector2 displacement, float height) {
        _sim = sim;
        _maxHeight;
    }

    public override void OnAdd(Warlock target) {
        _transformationId = target.Sprite.AddTransformation(1f);
        
        _displacementPerTick = displacement / duration.Ticks;
        
        _verticalAcceleration = - 8f * height / duration.Ticks.Squared();
        VerticalVelocity =  _verticalAcceleration * -0.5f * duration.Ticks;
        
        target.Jumping = true;
    }

    public override void OnRemove(Warlock target) {
        if (_transformationId != 0) {
            target.Sprite.RemoveTransformation(_transformationId);
        }

        target.Jumping = false;
    }
    
    protected override void OnUpdate(Warlock target) {
        Height += VerticalVelocity + _verticalAcceleration/2;
        VerticalVelocity += _verticalAcceleration;
        target.Sprite.ChangeTransformation(_transformationId, (Height + 1) * _heightScaleFactor);
        target.Position += _displacementPerTick;
    }
}