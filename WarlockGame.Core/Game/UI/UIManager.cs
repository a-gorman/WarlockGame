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
    /// <param name="acceptedCallback"> Callback with entered text called when text box is closed normally (with enter key)  </param>
    /// <param name="cancelledCallback"> Callback called when text box is closed in a way that does not indicate acceptance (such as clicking away) </param>
    public static void OpenTextPrompt(string promptText, Action<string> acceptedCallback, Action<string>? cancelledCallback = null) {
        var prompt = new TextPrompt(promptText, acceptedCallback, cancelledCallback);
        AddComponent(prompt);
        InputManager.AddTextConsumer(prompt);
    }

    public static void AddComponent(IUIComponent component) {
        Components.Add(component);
        Components.Sort((first, second) => second.Layer.CompareTo(first.Layer));
    }
}