using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter.Core.Game.Effect;

public interface IEffect
{
    bool IsExpired { get; }

    void Update();

    void Draw(SpriteBatch spriteBatch);
}