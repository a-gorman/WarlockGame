using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.UX.InputDevices;

/// <summary>
/// Gets input from XNA's GamePad
/// </summary>
class GamepadInput: IInputDevice {

    private readonly int _playerIndex;
    private GamePadState _gamePadState;
    private readonly IReadOnlyDictionary<Buttons,GamepadMapping> _mappings;
    private HashSet<InputAction> _actions = new();

    public GamepadInput(int playerIndex) {
        _playerIndex = playerIndex;
        
        _mappings = new List<GamepadMapping>
        {
            new() { DisplayValue = "A", Button = Buttons.RightTrigger, Action = InputAction.Select },
            new() { DisplayValue = "B", Button = Buttons.RightShoulder, Action = InputAction.Cancel },
            new() { DisplayValue = "Y", Button = Buttons.A, Action = InputAction.Spell1 },
            new() { DisplayValue = "Y", Button = Buttons.B, Action = InputAction.Spell2 },
            // new() { DisplayValue = "X", Button = Buttons.X, Action = null }
        }.ToDictionary(x => x.Button);
    }
    
    public IReadOnlySet<InputAction> GetInputActions() {
        return _actions;
    }

    public Vector2? Position => null;

    public Vector2? LeftStick { get; private set; } = null;
    public Vector2? RightStick { get; private set; } = null;
    
    public void Update() {
        _gamePadState = GamePad.GetState(_playerIndex);

        _actions.Clear();
        _mappings.Where(x => _gamePadState.IsButtonDown(x.Value.Button))
                 .Select(x => x.Value.Action)
                 .ForEach(x => _actions.Add(x));

        LeftStick = _gamePadState.ThumbSticks.Left;
        RightStick = _gamePadState.ThumbSticks.Right;
    }
    
    private struct GamepadMapping {
        public string DisplayValue { get; init; }
        public InputAction Action { get; init; }
        public Buttons Button { get; init; }
    }
}