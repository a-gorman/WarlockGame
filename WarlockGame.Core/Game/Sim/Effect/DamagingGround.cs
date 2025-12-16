using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace WarlockGame.Core.Game.Sim.Effect;

class DamagingGround : IEffect {
    public bool IsExpired { get; set; }

    public CircleF Shape { get; set; }
    public float DamagePerTick { get; }

    public bool Inverted { get; set; }
    
    private readonly Simulation _sim;

    public DamagingGround(Simulation sim, CircleF shape, float damagePerTick, bool inverted = false) {
        _sim = sim;
        Shape = shape;
        DamagePerTick = damagePerTick;
        Inverted = inverted;
    }

    public void Update() {
        foreach (var warlock in _sim.EntityManager.Warlocks) {
            if (Inverted ^ Shape.Contains(warlock.Position)) {
                warlock.Damage(DamagePerTick, null);
            }
        }

        SimDebug.VisualizeCircle(Shape.Radius, Shape.Position, Color.GhostWhite);
    }

    public void Draw(Vector2 location, SpriteBatch spriteBatch) { }
}