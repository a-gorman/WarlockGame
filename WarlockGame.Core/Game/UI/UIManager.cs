using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.UI;

/// <summary>
/// Manager for UI elements like text boxes, menus and status displays
/// </summary>
static class UIManager {
    private static SortedDictionary<int, List<IUIComponent>> _components = new();
    
    public static void Draw(SpriteBatch spriteBatch) {
        foreach (var component in _components.Values.SelectMany(x => x)) {
            component.Draw(spriteBatch);
        }
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
        AddComponent(new TextPrompt(promptText, onCloseCallback));
    }

    public static void AddComponent(IUIComponent component) {
        if (_components.ContainsKey(component.Layer)) {
            _components[component.Layer].Add(component);
        }
        else {
            _components[component.Layer] = new List<IUIComponent> { component };
        }
    }
    
}