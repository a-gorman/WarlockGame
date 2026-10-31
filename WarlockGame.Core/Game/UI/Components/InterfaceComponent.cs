using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

class InterfaceComponent {
    public ZLayer ZIndex { get; init; }
    
    /// <summary>
    /// Determines what is displayed on top of what among siblings in the interface tree.
    /// Higher layers are displayed on top of lower numbered layers.
    /// </summary>
    public int SiblingLayer { get; init; }

    public bool IsExpired { get; set; }
    public bool Disabled { get;
        set {
            if (value != field) {
                RequiresReindexing = true;
                field = value;
            }
        }
    }
    
    public ClickableState Clickable { get; set; } = ClickableState.Ignore;

    public bool Visible {
        get;
        set {
            if (value != field) {
                IsVisibilityDirty = true;
                DirtyIndexing();
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
        private set {
            if (value != field) {
                DirtyIndexing();
                field = value;
            }
        }
    }

    public Layout Layout {
        get;
        set {
            if (field != value) {
                DirtyIndexing();
                field = value;
            }
        }
    } = new();

    public bool RequiresReindexing { get; set; }
    public bool IsLayoutDirty { get; private set; } = true;
    protected bool IsVisibilityDirty { get; private set; } = true;

    protected bool WasMadeVisible => IsVisibilityDirty && Visible;
    
    public Vector2 RelativeLocation => BoundingBox.Location.ToVector2();

    private readonly List<InterfaceComponent> _components = [];
    public IReadOnlyList<InterfaceComponent> Components => _components;

    public virtual void OnLeftClick(Vector2 location) { }
    public virtual void OnRightClick(Vector2 location) { }

    public void DrawComponent(Vector2 location, SpriteBatch spriteBatch) {
        Draw(location, spriteBatch);
        DirtyIndexing();
    }
    
    protected virtual void Draw(Vector2 location, SpriteBatch spriteBatch) { }

    protected virtual void OnUpdate(in UIManager.UpdateArgs args) { }
    
    /// <summary>
    /// Update function called each frame before drawing any components.
    /// </summary>
    public void Update(ref readonly UIManager.UpdateArgs args) {
        OnUpdate(args);
    }
    
    public void AddComponent(InterfaceComponent component) {
        var originX = Math.Min(Math.Max(component.BoundingBox.X, 0), BoundingBox.Width);
        var originY = Math.Min(Math.Max(component.BoundingBox.Y, 0), BoundingBox.Height);
        var oppositeX = Math.Max(Math.Min(component.BoundingBox.Right, BoundingBox.Width), 0);
        var oppositeY = Math.Max(Math.Min(component.BoundingBox.Bottom, BoundingBox.Height), 0);

        var newBounds = new Rectangle(originX, originY, oppositeX - originX, oppositeY - originY);

        if (newBounds != component.BoundingBox) {
            Logger.Warning($"Nested component bounds are outside parent bounds. Adjusted '{component.BoundingBox}' to '{newBounds}'.", Logger.LogType.Interface);
            component.BoundingBox = newBounds;
        }
        
        _components.Add(component);
        _components.Sort((first, second) => second.SiblingLayer.CompareTo(first.SiblingLayer));
        component.OnAdd();
        DirtyIndexing();
    }
    
    public void RemoveComponent(InterfaceComponent component) {
        _components.Remove(component);
        component.OnRemove();
        DirtyIndexing();
    }
    
    public void RemoveAllComponents() {
        foreach (var component in _components) {
            component.OnRemove();
        }
        _components.Clear();
        DirtyIndexing();
    }
    
    public void RemoveAllComponents(Predicate<InterfaceComponent> predicate) {
        foreach (var component in _components) {
            if (predicate(component)) {
                component.OnRemove();
            }
        }
        _components.RemoveAll(predicate);
        DirtyIndexing();
    }

    public virtual void RefreshBounds(Rectangle parentBounds) {
        int width = Layout.HasLayoutFlags(Layout.LayoutFlags.AutoWidth)
            ? parentBounds.Width - 2 * Layout.Width // Interpret layout width as margin
            : Layout.Width;
        
        int height = Layout.HasLayoutFlags(Layout.LayoutFlags.AutoHeight)
            ? parentBounds.Height - 2 * Layout.Height // Interpret layout height as margin
            : Layout.Height;

        int xOffset = GetBaseXOffset(Layout, width, parentBounds) + Layout.Offset.X;
        int yOffset = GetBaseYOffset(Layout, height, parentBounds) + Layout.Offset.Y;
        
        var bounds = new Rectangle(xOffset, yOffset, width, height);

        if (Layout.HasLayoutFlags(Layout.LayoutFlags.RestrictBounds)) {
            bounds = parentBounds.AtOrigin().GetRelativeRectangle(bounds);
        }

        BoundingBox = bounds;
        IsLayoutDirty = false;
    }

    private static int GetBaseXOffset(in Layout layout, int width, Rectangle parentBounds) {
        if (layout.HasAlignmentFlags(AlignmentFlags.HorizontalCenter)) {
            return Math.Abs(parentBounds.Width - width) / 2;
        }
        if (layout.HasAlignmentFlags(AlignmentFlags.Left)) {
            return 0;
        }
        if (layout.HasAlignmentFlags(AlignmentFlags.Right)) {
            return Math.Abs(parentBounds.Width - width);
        }

        throw new Exception("Invalid horizontal flags");
    }
    
    private static int GetBaseYOffset(in Layout layout, int height, Rectangle parentBounds) {
        if (layout.HasAlignmentFlags(AlignmentFlags.VerticalCenter)) {
            return Math.Abs(parentBounds.Height - height) / 2;
        }
        if (layout.HasAlignmentFlags(AlignmentFlags.Top)) {
            return 0;
        }
        if (layout.HasAlignmentFlags(AlignmentFlags.Bottom)) {
            return Math.Abs(parentBounds.Height - height);
        }

        throw new Exception("Invalid vertical flags");
    }

    private void DirtyIndexing() {
        RequiresReindexing |= !Disabled;
    }

    public virtual void OnHover() { }

    public virtual void OnStopHover() { }

    public virtual void OnLostFocus() { }

    protected virtual void OnAdd() { }

    protected virtual void OnRemove() { }
    
    public enum ZLayer {
        Default,
        Tooltip
    }
}

readonly record struct Layout {
    public LayoutType Type { get; }
    public Alignment Alignment { get; }
    public Point Offset { get; }
    public int Width { get; }
    public int Height { get; }

    public Layout() {
        // By default, fill the parent
        Type = LayoutType.Margin;
        Alignment = Alignment.TopLeft;
    }
    
    private Layout(LayoutType type, 
        Alignment alignment = Alignment.TopLeft, 
        Point offset = new(), 
        int width = 0, 
        int height = 0) {
        Alignment = alignment;
        Offset = offset;
        Width = width;
        Height = height;
        Type = type;
    }
    
    public static Layout Fill() {
        return new Layout();
    }
    
    public static Layout WithMargin(int margin) {
        return new Layout(LayoutType.Margin, alignment: Alignment.Center, width: margin, height: margin);
    }
    
    public static Layout WithMargin(int widthMargin, int heightMargin) {
        return new Layout(LayoutType.Margin, alignment: Alignment.Center, width: widthMargin, height: heightMargin);
    }

    public static Layout WithHeight(int height, int heightOffset = 0, int widthMargin = 0, Alignment alignment = Alignment.Top) {
        return new Layout(LayoutType.WidthMargin,
            alignment: alignment,
            height: height,
            width: widthMargin,
            offset: new Point(0, heightOffset)
        );
    }

    public static Layout WithWidth(int width, int widthOffset = 0, int heightMargin = 0, Alignment alignment = Alignment.Left) {
        return new Layout(LayoutType.WidthMargin,
            alignment: alignment,
            height: heightMargin,
            width: width,
            offset: new Point(widthOffset, 0)
        );
    }
    
    public static Layout WithSize(int width, int height, Alignment alignment = Alignment.TopLeft) {
        return new Layout(LayoutType.ManualSize,
            alignment: alignment,
            width: width,
            height: height);
    }

    public static Layout WithBoundingBox(Rectangle boundingBox, Alignment alignment = Alignment.TopLeft) {
        return new Layout(LayoutType.ManualSize,
            alignment: alignment,
            offset: boundingBox.Location,
            width: boundingBox.Width,
            height: boundingBox.Height);
    }

    public static Layout WithBoundingBox(int x, int y, int width, int height, Alignment alignment = Alignment.TopLeft) {
        return new Layout(LayoutType.ManualSize,
            alignment: alignment,
            offset: new Point(x, y),
            width: width,
            height: height);
    }
    
    public static Layout WithUnrestrictedBounds(int x, int y, int width, int height, Alignment alignment = Alignment.TopLeft) {
        return new Layout(LayoutType.Unrestricted,
            alignment: alignment,
            offset: new Point(x, y),
            width: width,
            height: height);
    }

    public bool HasLayoutFlags(LayoutFlags flags) {
        return ((int)Type & (int)flags) == (int)flags;
    }
    
    public bool HasAlignmentFlags(AlignmentFlags flags) {
        return ((int)Alignment & (int)flags) == (int)flags;
    }
    
    public enum LayoutType {
        Margin           = LayoutFlags.AutoHeight | LayoutFlags.AutoWidth | LayoutFlags.RestrictBounds,
        HeightMargin     = LayoutFlags.AutoHeight | LayoutFlags.RestrictBounds,
        WidthMargin      = LayoutFlags.AutoWidth | LayoutFlags.RestrictBounds,
        ManualSize       = LayoutFlags.RestrictBounds,
        Unrestricted     = 0,
        Outside          = LayoutFlags.OutsideParent,
        OutsideNoOverlap = LayoutFlags.OutsideParent | LayoutFlags.RestrictBounds
    }

    [Flags]
    public enum LayoutFlags {
        AutoWidth      = 0b0001,
        AutoHeight     = 0b0010,
        OutsideParent  = 0b0100,
        RestrictBounds = 0b1000
    }
}

/// <summary>
/// Defines what happens when a click lands in this elment's bounding box and is checked while looking for
/// a valid interface element to handle a click.
/// </summary>
public enum ClickableState {
    /// <summary> Stop looking for clickable elements </summary>
    Unclickable,
    /// <summary> Check for a valid clickable sub-element, and if none found, call this element's onClick function and stop </summary>
    Clickable,
    /// <summary> Call this element's onClick function and then keep looking for a click handler in this element's sub-elements </summary>
    Notify,
    /// <summary> Skip this element and keep looking for a click handler in this element's sub-elements </summary>
    PassThrough,
    /// <summary> Skip this element entirely, including sub-elements </summary>
    Ignore
}