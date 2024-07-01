using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.UI;

public interface IUIComponent {
    /// <summary>
    /// Determines what is displayed on top of what.
    /// Higher layers are displayed on top of lower numbered layers.
    /// </summary>
    public int Layer { get; }
    /// <summary>
    /// The bounding box for determining if the component was clicked.
    /// Higher layers obscure clicks from lower layers.
    /// </summary>
    public Rectangle BoundingBox { get; }
    public void OnClick(Vector2 location);
    public void Draw(SpriteBatch spriteBatch);
    public event EventHandler OnClose;
}