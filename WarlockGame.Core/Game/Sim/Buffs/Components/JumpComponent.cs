using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Buffs.Components;

class JumpComponent : BuffComponent {
    public float Height { get; private set; }
    public float VerticalVelocity { get; private set; }

    private readonly float _maxHeight;
    private readonly Simulation _sim;
    private readonly Vector2 _displacement;

    private Vector2 _displacementPerTick;
    private float _verticalAcceleration;
    private int _transformationId;
    
    private const float HeightScaleFactor = 0.2f;
    
    public JumpComponent(Simulation sim, Vector2 displacement, float height) {
        _sim = sim;
        _displacement = displacement;
        _maxHeight = height;
    }

    public override void OnAdd(Buff buff, Warlock target) {
        _transformationId = target.Sprite.AddTransformation(1f);
        
        var duration = buff.Timer!.Value.TicksRemaining;
        
        _displacementPerTick = _displacement / duration;
        
        _verticalAcceleration = - 8f * _maxHeight / duration.Squared();
        VerticalVelocity =  _verticalAcceleration * -0.5f * duration;
        
        target.Jumping = true;
    }

    public override void OnRemove(Buff buff, Warlock target) {
        if (_transformationId != 0) {
            target.Sprite.RemoveTransformation(_transformationId);
        }

        target.Jumping = false;
    }
    
    public override void Update(Buff buff, Warlock target) {
        Height += VerticalVelocity + _verticalAcceleration/2;
        VerticalVelocity += _verticalAcceleration;
        target.Sprite.ChangeTransformation(_transformationId, (Height + 1) * HeightScaleFactor);
        target.Position += _displacementPerTick;
    }
}