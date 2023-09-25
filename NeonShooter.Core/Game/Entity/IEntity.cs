using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter.Core.Game.Entity;

internal interface IEntity
{
    void Update();
    void Draw(SpriteBatch spriteBatch);

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }
    public float Radius { get; init; }	// used for circular collision detection
    bool IsExpired { get; set; }
    
}