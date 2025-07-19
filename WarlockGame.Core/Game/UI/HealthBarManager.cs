using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.UI;

class HealthBarManager : InterfaceComponent {
    private const int VerticalOffset = 30;
    private const int Width = 80;
    private const int Height = 3;

    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        foreach (var warlock in WarlockGame.Instance.Simulation.EntityManager.Warlocks) {
            float filledProportion = warlock.Health / warlock.MaxHealth;
        
            var filledTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            filledTexture.SetData([Color.Lerp(Color.Red, Color.Green, filledProportion)]);
        
            var unfilledTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            unfilledTexture.SetData([Color.Black]);

            var position = warlock.Position;
            position.Y -= VerticalOffset;
        
            spriteBatch.Draw(unfilledTexture, new Rectangle((int) position.X - Width/2, (int) position.Y, Width, Height), Color.White);
            spriteBatch.Draw(filledTexture, new Rectangle((int) position.X - Width/2, (int) position.Y, (int)(Width * filledProportion), Height), Color.White);
        }
    }
}