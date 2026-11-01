using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class Container : InterfaceComponent {
    public Color? BackgroundColor { get; set; } = null;
    public Color? BorderColor { get; set; } = null;
    public int BorderWidth { get; set; } = 1;

    public Container(params InterfaceComponent[] interfaceComponents) {
        foreach (var component in interfaceComponents) {
            AddComponent(component);
        }
    }
    
    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (BackgroundColor != null) {
            spriteBatch.Draw(Art.Pixel, BoundingBox.WithOffset(location), BackgroundColor.Value);
        }
        
        if (BorderColor != null) {
            UIUitils.DrawHollowRectangle(spriteBatch, Art.Pixel, BoundingBox.WithOffset(location), BorderColor.Value, width: BorderWidth);
        }
    }
}