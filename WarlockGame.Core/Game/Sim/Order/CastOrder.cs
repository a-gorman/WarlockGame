using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Entities;

namespace WarlockGame.Core.Game.Sim.Order; 

class CastOrder: IOrder {
    private readonly Warlock _player;
    private readonly int _spellId;
    private readonly Vector2 _castDirection;
    
    public bool Finished { get; private set; }
    
    public CastOrder(int spellId, Vector2 castDirection, Warlock player) {
        _spellId = spellId;
        _castDirection = castDirection;
        _player = player;
    }

    public void Update() {
        _player.CastSpell(_spellId, _castDirection);
        Finished = true;
    }

    public void OnCancel() { }
    public void OnFinish() { }
}