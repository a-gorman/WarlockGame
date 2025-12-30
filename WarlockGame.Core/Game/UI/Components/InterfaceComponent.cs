using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

class InterfaceComponent {
    /// <summary>
    /// Determines what is displayed on top of what.
    /// Higher layers are displayed on top of lower numbered layers.
    /// </summary>
    public int Layer { get; set; }

    public bool IsExpired { get; set; }
    public ClickableState Clickable { get; set; } = ClickableState.Ignore;

    public bool Visible {
        get;
        set {
            if (value != field) {
                IsVisibilityDirty = true;
                field = value;
            }
        }
    } = true;

    /// <summary>
    /// The bounding box for determining if the component was clicked.
    /// Higher layers obscure clicks from lower layers.
    /// </summary>
    public Rectangle BoundingBox { 
        get;
        set {
            if (field != value) {
                IsBoundsDirty = true; 
                field = value;
            }
        } 
    }

    public Alignment? Alignment {
        get;
        set {
            if (field != value) {
                IsBoundsDirty = true; 
                field = value;
            }
        } 
    }

    protected bool IsBoundsDirty { get; set; } = true;
    protected bool IsVisibilityDirty { get; set; } = true;

    protected bool WasMadeVisible => IsVisibilityDirty && Visible;
    
    public Vector2 RelativeLocation => BoundingBox.Location.ToVector2();

    private readonly List<InterfaceComponent> _components = [];
    public IReadOnlyList<InterfaceComponent> Components => _components;

    public virtual void OnLeftClick(Vector2 location) { }
    public virtual void OnRightClick(Vector2 location) { }

    public void DrawComponent(Vector2 location, SpriteBatch spriteBatch, Vector2 parentSize) {
        Draw(location, spriteBatch);
        IsVisibilityDirty = false;
        IsBoundsDirty = false;
    }
    
    protected virtual void Draw(Vector2 location, SpriteBatch spriteBatch) { }

    /// <summary>
    /// Update function called each frame before drawing any components.
    /// </summary>
    public virtual void Update(ref readonly UIManager.UpdateArgs args) { }

    public void AddComponent(InterfaceComponent component) {
        if (component.BoundingBox.IsEmpty) {
            component.BoundingBox = BoundingBox.AtOrigin();
            Logger.Debug($"Default bounding box assigned for interface component. {component.BoundingBox}", Logger.LogType.Interface);
        } else {
            var originX = Math.Min(Math.Max(component.BoundingBox.X, 0), BoundingBox.Width);
            var originY = Math.Min(Math.Max(component.BoundingBox.Y, 0), BoundingBox.Height);
            var oppositeX = Math.Max(Math.Min(component.BoundingBox.Right, BoundingBox.Width), 0);
            var oppositeY = Math.Max(Math.Min(component.BoundingBox.Bottom, BoundingBox.Height), 0);

            var newBounds = new Rectangle(originX, originY, oppositeX - originX, oppositeY - originY);

            if (newBounds != component.BoundingBox) {
                Logger.Warning($"Nested component bounds are outside parent bounds. Adjusted '{component.BoundingBox}' to '{newBounds}'.", Logger.LogType.Interface);
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
    
    public void RemoveAllComponents(Predicate<InterfaceComponent> predicate) {
        foreach (var component in _components) {
            if (predicate(component)) {
                component.OnRemove();
            }
        }
        _components.RemoveAll(predicate);
    }

    public virtual void OnAdd() { }

    public virtual void OnRemove() { }

    private Rectangle CalculateBounds(Vector2 maxSize) {
        if (Alignment == null) {
            return BoundingBox;
        }
        
        float? leftBound = null;
        float? rightBound = null;
        float? topBound = null;
        float? bottomBound = null;

        if (Alignment & AlignmentFlags.FillHorizontal != 0) {
            leftBound = 0;
            rightBound = maxSize.X;
        } else if ((Alignment & AlignmentFlags.Left) != 0) {
            leftBound = 0;
        }
        else if ((Alignment & AlignmentFlags.Right) != 0) {
            rightBound = maxSize.X;
        }

        if ((Alignment & AlignmentFlags.Top) != 0) {
            topBound = 0;
        }
        if ((Alignment & AlignmentFlags.Bottom) != 0) {
            bottomBound = maxSize.Y;
        }
    }
}

[Flags]
public enum AlignmentFlags {
    Center = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Top = 1 << 2,
    Bottom = 1 << 3,
    Fill = Left | Right | Top | Bottom,
    BottomLeft = Left | Bottom,
    BottomRight = Right | Bottom,
    TopLeft = Left | Top,
    TopRight = Right | Top,
    FillHorizontal = Right | Left,
    FillVertical = Top | Bottom,
}

public enum ClickableState {
    Unclickable,
    Clickable,
    PassThrough,
    Ignore
}