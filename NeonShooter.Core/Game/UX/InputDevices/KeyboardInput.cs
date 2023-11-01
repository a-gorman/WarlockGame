using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace NeonShooter.Core.Game.UX.InputDevices;

public class KeyboardInput: IInputDevice {
    private static KeyboardState _keyboardState, _lastKeyboardState;

    private readonly IReadOnlyDictionary<InputAction, KeyMapping> _mappings;
    private readonly IReadOnlyDictionary<Keys, KeyMapping> _reverseMappings;
    private HashSet<Keys> _releasedKeys = new();
    
    public KeyboardInput() {
        var keyMappings = new List<KeyMapping>()
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
        _reverseMappings = keyMappings.ToDictionary(x => x.Key);
    }

    // Checks if a key was just pressed down
    public bool WasKeyPressed(Keys key) {
        return _lastKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key);
    }
    
    public bool WasKeyReleased(Keys key) {
        return _releasedKeys.Contains(key);
    }

    public bool WasActionKeyPressed(InputAction action) {
        if (_mappings.TryGetValue(action, out var mapping)) {
            return WasKeyPressed(mapping.Key);
        }

        return false;
    }

    public bool WasActionReleased(InputAction action) {
        if (_mappings.TryGetValue(action, out var mapping)) {
            return WasKeyReleased(mapping.Key);
        }

        return false;
    }

    public bool IsActionKeyDown(InputAction action) {
        if (_mappings.TryGetValue(action, out var mapping)) {
            return _keyboardState.IsKeyDown(mapping.Key);
        }

        return false;
    }

    public IReadOnlySet<InputAction> GetReleasedActions() {
        return _releasedKeys.Where(_reverseMappings.ContainsKey)
                            .Select(x => _reverseMappings[x].InputAction)
                            .ToHashSet();
    }

    public IReadOnlySet<InputAction> GetPressedActions() {
        return _keyboardState.GetPressedKeys()
                             .Where(_reverseMappings.ContainsKey)
                             .Except(_lastKeyboardState.GetPressedKeys())
                             .Select(x => _reverseMappings[x].InputAction)
                             .ToHashSet();
    }

    public IReadOnlySet<InputAction> GetHeldActions() {
        return _keyboardState.GetPressedKeys()
                             .Where(_reverseMappings.ContainsKey)
                             .Select(x => _reverseMappings[x].InputAction)
                             .ToHashSet();
    }

    public void Update() {
        _lastKeyboardState = _keyboardState;

        _keyboardState = Keyboard.GetState();

        _releasedKeys = _lastKeyboardState.GetPressedKeys().Except(_keyboardState.GetPressedKeys()).ToHashSet();
    }
    
    private struct KeyMapping {
        public required string DisplayValue;
        public required Keys Key;
        public required InputAction InputAction;
    }
}