using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace WarlockGame.Core.Game.Input.Devices;

[Obsolete]
public static class StaticKeyboardInput {
    private static KeyboardState _keyboardState, _lastKeyboardState;
    
    // Checks if a key was just pressed down
    public static bool WasKeyPressed(Keys key) {
        return WarlockGame.Instance.IsActive && _lastKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key);
    }
    
    public static bool IsKeyPressed(Keys key) {
        return WarlockGame.Instance.IsActive && _keyboardState.IsKeyDown(key);
    }

    public static void Update() {
        _lastKeyboardState = _keyboardState;

        _keyboardState = Keyboard.GetState();
    }
}