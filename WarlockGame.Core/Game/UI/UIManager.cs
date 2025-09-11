using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using ZLinq;

namespace WarlockGame.Core.Game.UI;

/// <summary>
/// Manager for UI elements like text boxes, menus and status displays
/// </summary>
static class UIManager {
    private static readonly List<InterfaceComponent> Components = new();
    
    public static void Draw(SpriteBatch spriteBatch) {
        
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        foreach (var component in Components) {
            DrawComponent(component, Vector2.Zero, spriteBatch);
        }
        spriteBatch.End();

        // draw the custom mouse cursor
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        var color = InputManager.SelectedSpellId is null ? Color.White : Color.Red;
        spriteBatch.Draw(Art.Pointer, StaticInput.MousePosition,  color);
        spriteBatch.End();
    }

    public static void Update() {
        Components.RemoveAll(x => x.IsExpired);
        foreach (var component in Components.AsValueEnumerable()) {
            component.Update();
        }
    }

    /// <summary>
    /// Opens a new text prompt, and displays it to the user
    /// </summary>
    /// <param name="promptText">The text to display to the user that explains the box</param>
    /// <param name="acceptedCallback"> Callback with entered text called when text box is closed normally (with enter key)  </param>
    /// <param name="cancelledCallback"> Callback called when text box is closed in a way that does not indicate acceptance (such as clicking away) </param>
    public static void OpenTextPrompt(string promptText, Action<string> acceptedCallback, Action<string>? cancelledCallback = null) {
        var prompt = new TextPrompt(promptText, acceptedCallback, cancelledCallback);
        AddComponent(prompt);
        InputManager.AddTextConsumer(prompt);
    }

    public static void AddComponent(InterfaceComponent component) {
        Components.Add(component);
        Components.Sort((first, second) => second.Layer.CompareTo(first.Layer));
        component.OnAdd();
    }
    
    public static bool HandleClick(Vector2 clickLocation) {
        foreach (var component in Components) {
            if (component.Clickable && ClickComponent(component, clickLocation)) {
                return true;
            }
        }

        return false;
    }

    private static void DrawComponent(InterfaceComponent component, Vector2 globalLocation, SpriteBatch spriteBatch) {
        component.Draw(globalLocation, spriteBatch);
        foreach (var nestedComponent in component.Components) {
            if (component.Visible) {
                DrawComponent(nestedComponent, globalLocation + component.BoundingBox.Location.ToVector2(), spriteBatch);
            }
        }
    }
    
    private static bool ClickComponent(InterfaceComponent component, Vector2 clickLocation) {
        if (!component.BoundingBox.Contains(clickLocation)) return false;
        
        foreach (var nestedComponent in component.Components) {
            if (nestedComponent.Clickable) {
                if(ClickComponent(nestedComponent, clickLocation - component.RelativeLocation)) {
                    return true;
                }
            }
        }

        return component.OnClick(clickLocation);
    }
}