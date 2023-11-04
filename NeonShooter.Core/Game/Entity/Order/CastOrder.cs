using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Spell;

namespace NeonShooter.Core.Game.Entity.Order; 

class CastOrder: IOrder {
    private readonly PlayerShip _player;
    private readonly WarlockSpell _spell;
    private readonly Vector2 _castDirection;
    
    private bool _active;

    public bool Finished { get; private set; }
    
    public CastOrder(WarlockSpell spell, Vector2 castDirection, PlayerShip player) {
        _spell = spell;
        _castDirection = castDirection;
        _player = player;
    }

    public void Update() {
        _player.CastSpell(_spell, _castDirection);
        Finished = true;
    }

    public void OnCancel() { }
    public void OnFinish() { }
}