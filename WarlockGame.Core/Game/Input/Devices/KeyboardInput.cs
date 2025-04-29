using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WarlockGame.Core.Game.Input.Devices;

public class KeyboardInput: IInputDevice {
    private static KeyboardState _keyboardState;
    private readonly IReadOnlyDictionary<Keys, InputAction> _mappings;
    public Vector2? Position => null;

    public KeyboardInput(Dictionary<Keys, InputAction> keyMappings) {
        _mappings = keyMappings;
    }

    public IReadOnlySet<InputAction> GetInputActions() {
        return _keyboardState.GetPressedKeys()
                             .Where(_mappings.ContainsKey)
                             .Select(x => _mappings[x])
                             .ToHashSet();
    }

    public void Update() {
        _keyboardState = Keyboard.GetState();
    }
}