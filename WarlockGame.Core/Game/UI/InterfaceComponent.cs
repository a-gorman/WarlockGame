using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.UI;

public class InterfaceComponent {
    /// <summary>
    /// Determines what is displayed on top of what.
    /// Higher layers are displayed on top of lower numbered layers.
    /// </summary>
    public int Layer { get; set; }
    public bool IsExpired { get; set; }
    public virtual bool Visible { get; set; } = true;
    
    /// <summary>
    /// The bounding box for determining if the component was clicked.
    /// Higher layers obscure clicks from lower layers.
    /// </summary>
    public virtual Rectangle BoundingBox { get; set; }

    public List<InterfaceComponent> Components { get; } = [];
    
    public virtual void OnClick(Vector2 location) { }

    public virtual void Draw(SpriteBatch spriteBatch) { }
}