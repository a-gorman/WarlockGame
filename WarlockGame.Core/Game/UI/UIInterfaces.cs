using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.UI;

public interface IInterfaceComponent {
    /// <summary>
    /// Determines what is displayed on top of what.
    /// Higher layers are displayed on top of lower numbered layers.
    /// </summary>
    public int Layer { get; }

    public void Draw(SpriteBatch spriteBatch);
    public bool IsExpired { get; }
}

public interface IClickableComponent : IInterfaceComponent {
    /// <summary>
    /// The bounding box for determining if the component was clicked.
    /// Higher layers obscure clicks from lower layers.
    /// </summary>
    public Rectangle BoundingBox { get; }
    public void OnClick(Vector2 location);
}