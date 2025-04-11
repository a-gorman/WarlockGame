using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Entity;

namespace WarlockGame.Core.Game.UI;

class HealthBarManager : IInterfaceComponent {

    public int Layer => 0;

    private const int VerticalOffset = 30;
    private const int Width = 80;
    private const int Height = 3;

    public bool Visible { get; set; } = true;
    public IEnumerable<IInterfaceComponent> Components { get; } = new List<IInterfaceComponent>();

    public void Draw(SpriteBatch spriteBatch) {
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

    public bool IsExpired => false;
}