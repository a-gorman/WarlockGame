using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace WarlockGame.Core.Game.Input.Devices;

[Obsolete]
public static class StaticKeyboardInput {
    private static KeyboardState _keyboardState, _lastKeyboardState;

    public static IReadOnlyDictionary<InputAction, KeyMapping> _mappings;

    static StaticKeyboardInput() {
        var keyMappings = new List<KeyMapping>
        {
            new() { DisplayValue = "Q", Key = Keys.Q, InputAction = InputAction.Spell1 },
            new() { DisplayValue = "W", Key = Keys.W, InputAction = InputAction.Spell2 },
            new() { DisplayValue = "E", Key = Keys.E, InputAction = InputAction.Spell3 },
            new() { DisplayValue = "R", Key = Keys.R, InputAction = InputAction.Spell4 },
            new() { DisplayValue = "F", Key = Keys.F, InputAction = InputAction.Spell5 }
        };

        _mappings = keyMappings.ToDictionary(x => x.InputAction);
    }

    // Checks if a key was just pressed down
    public static bool WasKeyPressed(Keys key) {
        return WarlockGame.Instance.IsActive && _lastKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key);
    }

    public static void Update() {
        _lastKeyboardState = _keyboardState;

        _keyboardState = Keyboard.GetState();
    }
}

public struct KeyMapping {
    public required string DisplayValue;
    public required Keys Key;
    public required InputAction InputAction;
}