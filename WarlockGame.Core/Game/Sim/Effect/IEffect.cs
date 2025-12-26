using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Effect;

public interface IEffect
{
    bool IsExpired { get; }

    void Update();

    void Draw(Vector2 viewOffset, SpriteBatch spriteBatch);
}