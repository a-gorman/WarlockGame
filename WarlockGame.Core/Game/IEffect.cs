using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game;

public interface IEffect
{
    bool IsExpired { get; }

    void Update();

    void Draw(SpriteBatch spriteBatch);
}