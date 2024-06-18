using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Graphics.Effect;

public interface IEffect
{
    bool IsExpired { get; }

    void Update();

    void Draw(SpriteBatch spriteBatch);
}