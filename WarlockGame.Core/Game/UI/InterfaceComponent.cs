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

    private readonly List<InterfaceComponent> _components = [];
    public IReadOnlyList<InterfaceComponent> Components => _components;

    public virtual void OnClick(Vector2 location) { }

    public virtual void Draw(SpriteBatch spriteBatch) { }

    public void AddComponent(InterfaceComponent component) {
        _components.Add(component);
        component.OnAdd();
    }

    public void RemoveComponent(InterfaceComponent component) {
        _components.Remove(component);
        component.OnRemove();
    }

    public virtual void OnAdd() { }

    public virtual void OnRemove() { }
}