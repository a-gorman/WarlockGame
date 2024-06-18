using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Entity;

public interface IEntity
{
    void Update();
    void Draw(SpriteBatch spriteBatch);

    Vector2 Position { get; set; }
    Vector2 Velocity { get; set; }
    float Orientation { get; set; }
    float Radius { get; init; }	// used for circular collision detection
    bool IsExpired { get; set; }
    
}