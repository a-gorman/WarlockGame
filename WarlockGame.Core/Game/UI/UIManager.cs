using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

/// <summary>
/// Manager for UI elements like text boxes, menus and status displays
/// </summary>
static class UIManager {
    private static readonly SortedDictionary<int, List<IUIComponent>> Components = new();
    private static readonly SortedDictionary<int, List<ITextInputComponent>> TextInputComponents = new();
    
    public static void Draw(SpriteBatch spriteBatch) {
        // Favor newer items
        foreach (var component in Components.Values.SelectMany(x => x.AsEnumerable().Reverse())) {
            component.Draw(spriteBatch);
        }
    }

    public static void OnTextInput(TextInputEventArgs args) {
        // Favor newer items
        TextInputComponents.Values.SelectMany(x => x.AsEnumerable().Reverse())
                           .FirstOrDefault()
                           ?.OnTextInput(args);
    }
    
    /// <summary>
    /// Opens a new text prompt, and displays it to the user
    /// TODO: This needs to block keyboard input from the game.
    /// </summary>
    /// <param name="promptText">The text to display to the user that explains the box</param>
    /// <param name="onCloseCallback">
    /// Callback called when text box is closed <br/>
    /// String param is the user entered text <br/>
    /// Boolean param indicates if the user closed accepted the input (with the enter key). False if the box was exited
    /// in any way that indicates the prompt was canceled (such as clicking away or pressing escape)
    /// </param>
    public static void OpenTextPrompt(string promptText, Action<string, bool> onCloseCallback) {
        var prompt = new TextPrompt(promptText, onCloseCallback);
        AddComponent(prompt);
    }

    public static void AddComponent(IUIComponent component) {
        Components.AddItemToNestedList(component.Layer, component);

        if (component is ITextInputComponent textInput) {
            TextInputComponents.AddItemToNestedList(component.Layer, textInput);
        }
        
        component.OnClose += RemoveComponent;
    }

    private static void RemoveComponent(object? sender, EventArgs eventArgs) {
        if (sender is not IUIComponent component) return;
        component.OnClose -= RemoveComponent;
        
        if (Components.TryGetValue(component.Layer, out var components)) {
            components.Remove(component);
        }
            
        if (component is ITextInputComponent textInput) {
            if (TextInputComponents.TryGetValue(component.Layer, out var textInputs)) {
                textInputs.Remove(textInput);
            }
        }
    }
}