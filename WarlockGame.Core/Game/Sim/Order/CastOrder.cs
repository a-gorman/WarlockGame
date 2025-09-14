using System;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Order; 

class CastOrder: IOrder {
    private readonly Warlock _caster;
    private readonly int _spellId;
    private readonly Vector2 _castDirection;
    private const float AngleTolerance = Single.Pi / 6;

    public readonly CastType Type;

    public enum CastType {
        Self,
        Location,
        Directional
    }

    
    public bool Finished { get; private set; }
    
    public CastOrder(int spellId, Vector2 castDirection, Warlock caster, CastType type) {
        _spellId = spellId;
        _castDirection = castDirection;
        _caster = caster;
        Type = type;
    }

    public void Update() {
        if (Type == CastType.Directional) {
            var targetAngle = _castDirection.ToAngle();
            if(Math.Abs(_caster.Orientation - targetAngle) > AngleTolerance)
            {
                _caster.DesiredOrientation = targetAngle;
                return;
            }
        }

        if (Type == CastType.Location) {
            var displacement = _caster.Position - _castDirection;
            var targetOrientation = displacement.ToAngle();
            if (Math.Abs(_caster.Orientation - _castDirection.ToAngle()) > AngleTolerance) {
                _caster.DesiredOrientation = targetOrientation;
                return;
            }
        }
        
        _caster.CastSpell(_spellId, _castDirection);
        Finished = true;
    }

    public void OnCancel() {
        _caster.DesiredOrientation = null;
    }

    public void OnFinish() {
        _caster.DesiredOrientation = null;
    }
}