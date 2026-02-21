using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Effect;

class ShockwaveEffect: IEffect {
    public Vector2 Position { get; set; }
    private GameTimer _timer;
    private SpellContext _context;
    private readonly Vector2 _velocity;
    private readonly Vector2 _pushVector;
    private readonly int _radius = 60;

    public bool IsExpired { get; set; }
    
    public ShockwaveEffect(SpellContext context, Vector2 position, int distance, Vector2 velocity, float pushAmount) {
        _context = context;
        _velocity = velocity;
        Position = position;
        _timer = GameTimer.FromTicks((int)(distance / velocity.Length()));
        _pushVector = velocity.WithLength(pushAmount);
    }

    public void Update() {
        _timer = _timer.Decremented();
        if (_timer.IsExpired) {
            IsExpired = true;
            return;
        }

        var entities = _context.EntityManager.GetNearbyEntities(Position, _radius);
        foreach (var entity in entities) {
            if (entity.Id != _context.Caster.Id && entity is Warlock or Projectile) {
                entity.Push(_pushVector);
            }
        }
        
        Position += _velocity;
    }
    
    public void Draw(Vector2 viewOffset, SpriteBatch spriteBatch) {
        spriteBatch.DrawCircle(new CircleF(Position + viewOffset, _radius), 30, Color.WhiteSmoke);
    }
}