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
    public bool Clickable { get; set; } = false;
    public virtual bool Visible { get; set; } = true;
    
    /// <summary>
    /// The bounding box for determining if the component was clicked.
    /// Higher layers obscure clicks from lower layers.
    /// </summary>
    public virtual Rectangle BoundingBox { get; set; }

    public Vector2 RelativeLocation => BoundingBox.Location.ToVector2();

    private readonly List<InterfaceComponent> _components = [];
    public IReadOnlyList<InterfaceComponent> Components => _components;

    public virtual bool OnClick(Vector2 location) { return false; }

    public virtual void Draw(Vector2 location, SpriteBatch spriteBatch) { }

    public virtual void Update() { }

    public void AddComponent(InterfaceComponent component) {
        _components.Add(component);
        _components.Sort((first, second) => second.Layer.CompareTo(first.Layer));
        component.OnAdd();
    }

    public void RemoveComponent(InterfaceComponent component) {
        _components.Remove(component);
        component.OnRemove();
    }

    public virtual void OnAdd() { }

    public virtual void OnRemove() { }
}