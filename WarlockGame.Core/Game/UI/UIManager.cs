using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

/// <summary>
/// Manager for UI elements like text boxes, menus and status displays
/// </summary>
static class UIManager {
    private static readonly List<IUIComponent> Components = new();
    
    public static void Draw(SpriteBatch spriteBatch) {
        foreach (var component in Components) {
            component.Draw(spriteBatch);
        }
    }

    public static void Update() {
        Components.RemoveAll(x => x.IsExpired);
    }
    
    /// <summary>
    /// Opens a new text prompt, and displays it to the user
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
        InputManager.AddTextConsumer(prompt);
    }

    public static void AddComponent(IUIComponent component) {
        Components.Add(component);
        Components.Sort((first, second) => second.Layer.CompareTo(first.Layer));
    }
}