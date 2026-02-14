using System;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Order; 

class CastOrder: IOrder {
    private readonly Warlock _caster;
    private readonly int _spellId;
    private readonly Vector2 _castTarget;
    private const float AngleTolerance = Single.Pi / 12;

    public readonly CastType Type;

    public enum CastType {
        Self,
        Location,
        Directional
    }

    
    public bool Finished { get; private set; }
    
    public CastOrder(int spellId, Vector2 castTarget, Warlock caster, CastType type) {
        _spellId = spellId;
        _castTarget = castTarget;
        _caster = caster;
        Type = type;
    }

    public void Update() {
        if (Type == CastType.Directional) {
            var targetOrientation = _castTarget.ToAngle();
            if(Math.Abs(_caster.Orientation - targetOrientation) > AngleTolerance) {
                _caster.DesiredOrientation = targetOrientation;
                return;
            }
        }

        if (Type == CastType.Location) {
            var displacement = _castTarget - _caster.Position;
            var targetOrientation = displacement.ToAngle();
            if (Math.Abs(_caster.Orientation - targetOrientation) > AngleTolerance) {
                _caster.DesiredOrientation = targetOrientation;
                return;
            }
        }
        
        _caster.CastSpell(_spellId, _castTarget);
        Finished = true;
    }

    public void OnCancel() {
        _caster.DesiredOrientation = null;
    }

    public void OnFinish() {
        _caster.DesiredOrientation = null;
    }
}