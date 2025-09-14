using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Sim.Perks;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

class HealthBarManager : InterfaceComponent {
    private const int VerticalOffset = 30;
    private const int Width = 80;
    private const int Height = 3;

    private readonly Simulation _sim;

    public HealthBarManager(Simulation sim) {
        _sim = sim;
    }

    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        foreach (var warlock in _sim.EntityManager.Warlocks) {
            var opacity = CalculateOpacity(warlock);
            float filledProportion = warlock.Health / warlock.MaxHealth;

            var filledTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            filledTexture.SetData([Color.Lerp(Color.Red * opacity, Color.Green * opacity, filledProportion)]);

            var unfilledTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            unfilledTexture.SetData([Color.Black * opacity]);

            var position = warlock.Position;
            position.Y -= VerticalOffset;

            spriteBatch.Draw(unfilledTexture,
                new Rectangle((int)position.X - Width / 2, (int)position.Y, Width, Height), Color.White);
            spriteBatch.Draw(filledTexture,
                new Rectangle((int)position.X - Width / 2, (int)position.Y, (int)(Width * filledProportion), Height),
                Color.White);
        }
    }

    private float CalculateOpacity(Warlock warlock) {
        if (warlock.PlayerId != PlayerManager.LocalPlayerId) {
            var perk = _sim.PerkManager.GetPlayerPerk(warlock.PlayerId!.Value, Perk.PerkType.Invisibility);
            if (perk is InvisibilityPerk invisibilityPerk) {
                var pos = _sim.EntityManager.GetWarlockByForceId(PlayerManager.LocalPlayerId!.Value)?.Position;
                if (pos != null) {
                    return invisibilityPerk.CalculateVisibility((warlock.Position - pos.Value).Length());
                }
            }
        }

        return 1;
    }
}