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
            new() { DisplayValue = "W", Key = Keys.W, InputAction = InputAction.MoveUp },
            new() { DisplayValue = "S", Key = Keys.S, InputAction = InputAction.MoveDown },
            new() { DisplayValue = "A", Key = Keys.A, InputAction = InputAction.MoveLeft },
            new() { DisplayValue = "D", Key = Keys.D, InputAction = InputAction.MoveRight },
            new() { DisplayValue = "Q", Key = Keys.Q, InputAction = InputAction.Spell1 },
            new() { DisplayValue = "E", Key = Keys.E, InputAction = InputAction.Spell2 },
            new() { DisplayValue = "R", Key = Keys.R, InputAction = InputAction.Spell3 },
            new() { DisplayValue = "F", Key = Keys.F, InputAction = InputAction.Spell4 }
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