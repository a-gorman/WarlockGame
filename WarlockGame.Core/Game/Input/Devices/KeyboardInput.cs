using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WarlockGame.Core.Game.Input.Devices;

public class KeyboardInput: IInputDevice {
    private static KeyboardState _keyboardState;
    private readonly IReadOnlyDictionary<Keys, KeyMapping> _mappings;
    public Vector2? Position => null;
    public Vector2? LeftStick => null;
    public Vector2? RightStick => null;

    public KeyboardInput() {
        _mappings = new List<KeyMapping>()
        {
            new() { DisplayValue = "W", Key = Keys.W, InputAction = InputAction.MoveUp },
            new() { DisplayValue = "S", Key = Keys.S, InputAction = InputAction.MoveDown },
            new() { DisplayValue = "A", Key = Keys.A, InputAction = InputAction.MoveLeft },
            new() { DisplayValue = "D", Key = Keys.D, InputAction = InputAction.MoveRight },
            new() { DisplayValue = "Q", Key = Keys.Q, InputAction = InputAction.Spell1 },
            new() { DisplayValue = "E", Key = Keys.E, InputAction = InputAction.Spell2 },
            new() { DisplayValue = "R", Key = Keys.R, InputAction = InputAction.Spell3 },
            new() { DisplayValue = "F", Key = Keys.F, InputAction = InputAction.Spell4 },
            new() { DisplayValue = "G", Key = Keys.G, InputAction = InputAction.Spell5 },
            new() { DisplayValue = "Escape", Key = Keys.Escape, InputAction = InputAction.Exit },
            new() { DisplayValue = "P", Key = Keys.P, InputAction = InputAction.Pause },
            new() { DisplayValue = "Enter", Key = Keys.Enter, InputAction = InputAction.OpenCommandInput },
        }.ToDictionary(x => x.Key);
    }

    public IReadOnlySet<InputAction> GetInputActions() {
        return _keyboardState.GetPressedKeys()
                             .Where(_mappings.ContainsKey)
                             .Select(x => _mappings[x].InputAction)
                             .ToHashSet();
    }

    public void Update() {
        _keyboardState = Keyboard.GetState();
    }
    
    private struct KeyMapping {
        public required string DisplayValue;
        public required Keys Key;
        public required InputAction InputAction;
    }
}