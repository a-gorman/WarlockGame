using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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
    public bool Disabled { get; set; }
    
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
        private set;
    }

    public Layout Layout {
        get;
        set {
            if (field != value) {
                IsLayoutDirty = true;
                field = value;
            }
        }
    } = new();

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
        IsVisibilityDirty = false;
    }
    
    protected virtual void Draw(Vector2 location, SpriteBatch spriteBatch) { }

    /// <summary>
    /// Update function called each frame before drawing any components.
    /// </summary>
    public virtual void Update(ref readonly UIManager.UpdateArgs args) { }
    
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
        _components.Sort((first, second) => second.Layer.CompareTo(first.Layer));
        component.OnAdd();
    }

    public virtual void RefreshBounds(Rectangle parentBounds) {
        switch (Layout.Type) {
            case Layout.LayoutType.Margin:
                BoundingBox = parentBounds
                    .AtLocation(Layout.Offset)
                    .WithMargin(Layout.Width, Layout.Height);
                break;
            case Layout.LayoutType.Manual:
                int xOffset;
                int yOffset;
                switch (Layout.Origin) {
                    case Layout.Alignment.TopLeft:
                        xOffset = Layout.Offset.X;
                        yOffset = Layout.Offset.Y;
                        break;
                    case Layout.Alignment.TopCenter:
                        xOffset = CenterOffset(Layout.Offset.X, Layout.Width, parentBounds.Width);
                        yOffset = Layout.Offset.Y;
                        break;
                    case Layout.Alignment.TopRight:
                        xOffset = OppositeSideOffset(Layout.Offset.X, Layout.Width, parentBounds.Width);
                        yOffset = Layout.Offset.Y;
                        break;
                    case Layout.Alignment.CenterLeft:
                        xOffset = Layout.Offset.X;
                        yOffset = CenterOffset(Layout.Offset.Y, Layout.Height, parentBounds.Height);
                        break;
                    case Layout.Alignment.Center:
                        xOffset = CenterOffset(Layout.Offset.X, Layout.Width, parentBounds.Width);
                        yOffset = CenterOffset(Layout.Offset.Y, Layout.Height, parentBounds.Height);
                        break;
                    case Layout.Alignment.CenterRight:
                        xOffset = OppositeSideOffset(Layout.Offset.X, Layout.Width, parentBounds.Width);
                        yOffset = CenterOffset(Layout.Offset.Y, Layout.Height, parentBounds.Height);
                        break;
                    case Layout.Alignment.BottomLeft:
                        xOffset = Layout.Offset.X;
                        yOffset = OppositeSideOffset(Layout.Offset.Y, Layout.Height, parentBounds.Height);
                        break;
                    case Layout.Alignment.BottomCenter:
                        xOffset = CenterOffset(Layout.Offset.X, Layout.Width, parentBounds.Width);
                        yOffset = OppositeSideOffset(Layout.Offset.Y, Layout.Height, parentBounds.Height);
                        break;
                    case Layout.Alignment.BottomRight:
                        xOffset = OppositeSideOffset(Layout.Offset.X, Layout.Width, parentBounds.Width);
                        yOffset = OppositeSideOffset(Layout.Offset.Y, Layout.Height, parentBounds.Height);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(UI.Components.Layout.Type), Layout.Type, null);
                }

                BoundingBox = parentBounds.AtOrigin().GetRelativeRectangle(xOffset, yOffset, Layout.Width, Layout.Height);
                break;
        }

        IsLayoutDirty = false;
        return;

        // Nested functions
        int CenterOffset(int offset, int childSideLength, int parentSideLength) {
            return Math.Abs(parentSideLength - (childSideLength - offset)) / 2;
        }

        int OppositeSideOffset(int offset, int childSideLength, int parentSideLength) {
            return Math.Abs(parentSideLength - (childSideLength - offset));
        }
    }

    public void RemoveComponent(InterfaceComponent component) {
        _components.Remove(component);
        component.OnRemove();
        component.Disabled = true;
    }
    
    public void RemoveAllComponents() {
        foreach (var component in _components) {
            component.OnRemove();
            component.Disabled = true;
        }
        _components.Clear();
    }
    
    public void RemoveAllComponents(Predicate<InterfaceComponent> predicate) {
        foreach (var component in _components) {
            if (predicate(component)) {
                component.OnRemove();
                component.Disabled = true;
            }
        }
        _components.RemoveAll(predicate);
    }

    public virtual void OnLostFocus() { }

    protected virtual void OnAdd() { }

    protected virtual void OnRemove() { }
}

public record struct Layout {
    public LayoutType Type { get; set; }
    public Alignment Origin { get; set; }
    public Point Offset { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Layout() {
        Type = LayoutType.Margin;
    }
    
    private Layout(LayoutType type, 
        Alignment alignment = Alignment.TopLeft, 
        Point offset = new(), 
        int width = 0, 
        int height = 0) {
        Origin = alignment;
        Offset = offset;
        Width = width;
        Height = height;
        Type = type;
    }
    
    public static Layout Fill() {
        return new Layout();
    }
    
    public static Layout WithMargin(int margin) {
        return new Layout(LayoutType.Margin, width: margin, height: margin);
    }
    
    public static Layout WithMargin(int widthMargin, int heightMargin) {
        return new Layout(LayoutType.Margin, width: widthMargin, height: heightMargin);
    }
    
    public static Layout WithSize(int width, int height, Alignment alignment = Alignment.TopLeft) {
        return new Layout(LayoutType.Manual,
            alignment: alignment,
            width: width,
            height: height);
    }

    public static Layout WithBoundingBox(Rectangle boundingBox, Alignment alignment = Alignment.TopLeft) {
        return new Layout(LayoutType.Manual,
            alignment: alignment,
            offset: boundingBox.Location,
            width: boundingBox.Width,
            height: boundingBox.Height);
    }

    public static Layout WithBoundingBox(int x, int y, int width, int height, Alignment alignment = Alignment.TopLeft) {
        return new Layout(LayoutType.Manual,
            alignment: alignment,
            offset: new Point(x, y),
            width: width,
            height: height);
    }

    public enum Alignment {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum LayoutType {
        Manual,
        Margin
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