using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using NeonShooter.Core.Game.UX;

namespace NeonShooter.Core.Game.UX; 

public static class KeyboardInput {

    private static KeyboardState _keyboardState, _lastKeyboardState;

    
    public static IReadOnlyDictionary<InputAction, KeyMapping> _mappings = new List<KeyMapping>()
    {
        new() { DisplayValue = "W", Key = Keys.W, InputAction = InputAction.MoveUp },
        new() { DisplayValue = "S", Key = Keys.S, InputAction = InputAction.MoveDown },
        new() { DisplayValue = "A", Key = Keys.A, InputAction = InputAction.MoveLeft },
        new() { DisplayValue = "D", Key = Keys.D, InputAction = InputAction.MoveRight },
        new() { DisplayValue = "Q", Key = Keys.Q, InputAction = InputAction.Spell1 },
        new() { DisplayValue = "E", Key = Keys.E, InputAction = InputAction.Spell2 },
        new() { DisplayValue = "R", Key = Keys.R, InputAction = InputAction.Spell3 },
        new() { DisplayValue = "F", Key = Keys.F, InputAction = InputAction.Spell4 },
    }.ToDictionary(x => x.InputAction, x => x);

    
    // Checks if a key was just pressed down
    public static bool WasKeyPressed(Keys key)
    {
        return _lastKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key);
    }

    public static bool WasActionKeyPressed(InputAction action) {
        if (_mappings.TryGetValue(action, out var mapping)) {
            return WasKeyPressed(mapping.Key);
        }

        return false;
    }

    public static bool IsActionKeyDown(InputAction action) {
        if (_mappings.TryGetValue(action, out var mapping)) {
            return _keyboardState.IsKeyDown(mapping.Key);
        }

        return false;
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