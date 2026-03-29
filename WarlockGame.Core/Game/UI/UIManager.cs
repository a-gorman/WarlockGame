using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.UI.Components;

namespace WarlockGame.Core.Game.UI;

/// <summary>
/// Manager for UI elements like text boxes, menus and status displays
/// </summary>
// ReSharper disable once InconsistentNaming
static class UIManager {
    private static readonly UpdateArgs.GlobalProps GlobalProps = new();

    private static InterfaceComponent _view = new() { Clickable = ClickableState.PassThrough };

    private static InterfaceComponent _focusedComponent = _view;
    
    public static void Initialize() {
        _view.Layout = Layout.WithSize(Configuration.ScreenWidth, Configuration.ScreenHeight);
    }
    
    public static void Draw(SpriteBatch spriteBatch) {
        
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        DrawComponent(_view, Vector2.Zero, spriteBatch);
        spriteBatch.End();

        // draw the custom mouse cursor
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        var color = InputManager.SelectedSpellId is null ? Color.White : Color.Red;
        spriteBatch.Draw(Art.Pointer, StaticInput.MousePosition,  color);
        spriteBatch.End();
    }

    public static void Update(InputManager.InputState inputState) {
        GlobalProps.MouseInBounds = new Rectangle(Point.Zero, Simulation.ArenaSize.ToPoint()).Contains(inputState.GetMousePosition());
        GlobalProps.InputState = inputState;
        
        var args = new UpdateArgs {
            MousePosition = GlobalProps.MousePosition,
            Global = GlobalProps
        };
        
        UpdateComponent(_view, null, ref args, false);
    }

    private static void UpdateComponent(InterfaceComponent component, InterfaceComponent? parent, ref readonly UpdateArgs args, bool parentBoundsRefreshed) {
        if (component.Disabled)
            return;
        
        var mousePos = args.MousePosition;
        if (mousePos != null && component.BoundingBox.Contains(mousePos.Value)) {
            mousePos -= component.BoundingBox.Location.ToVector2();
        } else {
            mousePos = null;
        }

        var updateArgs = args with { MousePosition = mousePos };
        component.Update(ref updateArgs);
        if (parentBoundsRefreshed || component.IsLayoutDirty) {
            component.RefreshBounds(parent?.BoundingBox ?? new Rectangle(0, 0, Configuration.ScreenWidth, Configuration.ScreenHeight));
            parentBoundsRefreshed = true;
        }
        foreach (var nestedComponent in component.Components) {
            UpdateComponent(nestedComponent, component, ref updateArgs, parentBoundsRefreshed);
        }
        component.RemoveAllComponents(x => x.IsExpired);
    }

    /// <summary>
    /// Opens a new text prompt, and displays it to the user
    /// </summary>
    /// <param name="promptText">The text to display to the user that explains the box</param>
    /// <param name="acceptedCallback">Callback with entered text called when text box is closed normally (with enter key)  </param>
    /// <param name="cancelledCallback">Callback called when text box is closed in a way that does not indicate acceptance (such as clicking away) </param>
    public static void OpenTextPrompt(string promptText, Action<string> acceptedCallback, Action<string>? cancelledCallback = null) {
        var prompt = new TextPrompt(promptText, acceptedCallback, cancelledCallback);
        AddComponent(prompt);
        InputManager.AddTextConsumer(prompt);
        FocusComponent(prompt);
    }

    public static void RegisterTextConsumer(ITextInputConsumer component) {
        InputManager.AddTextConsumer(component);
    }

    public static void RemoveTextConsumer(ITextInputConsumer component) {
        InputManager.RemoveTextConsumer(component);
    }

    public static void AddComponent(InterfaceComponent component) {
        _view.AddComponent(component);
    }
    
    public static void HandleLeftClick(Vector2 clickLocation) {
        LeftClickComponent(_view, clickLocation);
    }
    
    public static void HandleRightClick(Vector2 clickLocation) {
        RightClickComponent(_view, clickLocation);
    }

    private static void DrawComponent(InterfaceComponent component, Vector2 globalLocation, SpriteBatch spriteBatch) {
        if (component.Visible && !component.Disabled) {
            component.DrawComponent(globalLocation, spriteBatch);
            for (var index = component.Components.Count - 1; index >= 0; index--) {
                var nestedComponent = component.Components[index];
                DrawComponent(nestedComponent, globalLocation + component.BoundingBox.Location.ToVector2(),
                    spriteBatch);
            }
        }
    }
    
    private static bool LeftClickComponent(InterfaceComponent component, Vector2 clickLocation) {
        if (component.Disabled || !component.Visible || !component.BoundingBox.Contains(clickLocation)) return false;
        var relativeClick = clickLocation - component.RelativeLocation;
        switch (component.Clickable) {
            case ClickableState.PassThrough:
                foreach (var nestedComponent in component.Components) {
                    var consumed = LeftClickComponent(nestedComponent, relativeClick);
                    if (consumed) return true;
                }
                return false;
            case ClickableState.Clickable:
                foreach (var nestedComponent in component.Components) {
                    var consumed = LeftClickComponent(nestedComponent, relativeClick);
                    if (consumed) return true;
                }
                component.OnLeftClick(relativeClick);
                FocusComponent(component);
                return true;
            case ClickableState.Unclickable:
                return true;
            case ClickableState.Ignore:
                return false;
            case ClickableState.Notify:
                foreach (var nestedComponent in component.Components) {
                    var consumed = LeftClickComponent(nestedComponent, relativeClick);
                    if (consumed) return true;
                }
                component.OnLeftClick(relativeClick);
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private static bool RightClickComponent(InterfaceComponent component, Vector2 clickLocation) {
        if (component.Disabled || !component.Visible || !component.BoundingBox.Contains(clickLocation)) return false;
        switch (component.Clickable) {
            case ClickableState.PassThrough:
                foreach (var nestedComponent in component.Components) {
                    var consumed = RightClickComponent(nestedComponent, clickLocation - component.RelativeLocation);
                    if (consumed) return true;
                }
                return false;
            case ClickableState.Clickable:
                foreach (var nestedComponent in component.Components) {
                    var consumed = RightClickComponent(nestedComponent, clickLocation - component.RelativeLocation);
                    if (consumed) return true;
                }
                component.OnRightClick(clickLocation);
                FocusComponent(component);
                return true;
            case ClickableState.Unclickable:
                return true;
            case ClickableState.Ignore:
                return false;
            case ClickableState.Notify:
                foreach (var nestedComponent in component.Components) {
                    var consumed = RightClickComponent(nestedComponent, clickLocation - component.RelativeLocation);
                    if (consumed) return true;
                }
                component.OnRightClick(clickLocation);
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void FocusComponent(InterfaceComponent component) {
        if (component != _focusedComponent) {
            _focusedComponent.OnLostFocus();
            _focusedComponent = component;
        }
    }
    
    public struct UpdateArgs {
        public required Vector2? MousePosition { get; init; }

        public required GlobalProps Global { get; init; }

        public class GlobalProps {
            public bool MouseInBounds { get; set; }
            public Vector2 MousePosition => InputState.GetMousePosition();
            public InputManager.InputState InputState { get; set; } = null!;
        }
    }
}