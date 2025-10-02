using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

public class InterfaceComponent {
    /// <summary>
    /// Determines what is displayed on top of what.
    /// Higher layers are displayed on top of lower numbered layers.
    /// </summary>
    public int Layer { get; set; }

    public bool IsExpired { get; set; }
    public ClickableState Clickable { get; set; } = ClickableState.Unclickable;
    public virtual bool Visible { get; set { IsDirty = true; field = value; } } = true;
    public bool IsDirty { get; set; } = true;

    /// <summary>
    /// The bounding box for determining if the component was clicked.
    /// Higher layers obscure clicks from lower layers.
    /// </summary>
    public virtual Rectangle BoundingBox { get; set { IsDirty = true; field = value; } }

    public Vector2 RelativeLocation => BoundingBox.Location.ToVector2();

    private readonly List<InterfaceComponent> _components = [];
    public IReadOnlyList<InterfaceComponent> Components => _components;

    public virtual void OnLeftClick(Vector2 location) { }
    public virtual void OnRightClick(Vector2 location) { }

    public virtual void Draw(Vector2 location, SpriteBatch spriteBatch) { IsDirty = false; }

    public virtual void Update() { }

    public void AddComponent(InterfaceComponent component) {
        if (component.BoundingBox.IsEmpty) {
            component.BoundingBox = BoundingBox.AtOrigin();
            Logger.Debug($"Default bounding box assigned for interface component. {component.BoundingBox}");
        } else {
            var originX = Math.Min(Math.Max(component.BoundingBox.X, 0), BoundingBox.Width);
            var originY = Math.Min(Math.Max(component.BoundingBox.Y, 0), BoundingBox.Height);
            var oppositeX = Math.Max(Math.Min(component.BoundingBox.Right, BoundingBox.Width), 0);
            var oppositeY = Math.Max(Math.Min(component.BoundingBox.Bottom, BoundingBox.Height), 0);

            var newBounds = new Rectangle(originX, originY, oppositeX - originX, oppositeY - originY);

            if (newBounds != component.BoundingBox) {
                Logger.Warning($"Nested component bounds are outside parent bounds. Adjusted '{component.BoundingBox}' to '{newBounds}'.");
                component.BoundingBox = newBounds;
            }
        }
        
        _components.Add(component);
        _components.Sort((first, second) => second.Layer.CompareTo(first.Layer));
        component.OnAdd();
    }

    public void RemoveComponent(InterfaceComponent component) {
        _components.Remove(component);
        component.OnRemove();
    }
    
    public void RemoveAllComponents() {
        foreach (var component in _components) {
            component.OnRemove();
        }
        _components.Clear();
    }

    public virtual void OnAdd() { }

    public virtual void OnRemove() { }
}

public enum ClickableState {
    Unclickable,
    Clickable,
    PassThrough,
    Ignore
}