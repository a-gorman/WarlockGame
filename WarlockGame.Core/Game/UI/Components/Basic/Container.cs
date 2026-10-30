using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class Container : InterfaceComponent {
    public Color? Color { get; set; } = null;

    public Container(params InterfaceComponent[] interfaceComponents) {
        foreach (var component in interfaceComponents) {
            AddComponent(component);
        }
    }
    
    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (Color != null) {
            spriteBatch.Draw(Art.Pixel, BoundingBox.WithOffset(location), Color.Value);
        }
    }
}