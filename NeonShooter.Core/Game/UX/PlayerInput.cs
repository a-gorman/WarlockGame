using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Util;
using NeonShooter.Core.Game.UX.InputDevices;

namespace NeonShooter.Core.Game.UX; 

/// <summary>
/// Handles input from a particular player
/// </summary>
class PlayerInput {

    private readonly Dictionary<InputAction, HashSet<Action>> _onPressedActions = new();
    private readonly Dictionary<InputAction, HashSet<Action>> _whilePressedActions = new();
    
    private static InputState _inputState =  new(new HashSet<InputAction>());
    private static InputState _lastInputState =  new(new HashSet<InputAction>());

    // Input devices to use for this player. For example Keyboard+Mouse or gamepad
    private readonly List<IInputDevice> _inputDevices = new();

    public PlayerInput() {
        var keyboardInput = new KeyboardInput();
        _inputDevices.Add(keyboardInput);
        _inputDevices.Add(new MouseInput());
        InputDeviceManager.Add(keyboardInput);
    }
    
    public bool IsActionKeyDown(InputAction action) => _inputState.Actions.Contains(action);
    
    public void Update() {
        CreateInputState();
        ProcessPlayerActions();
    }

    private void CreateInputState() {
        (_inputState, _lastInputState) = (_lastInputState, _inputState);
        
        _inputState.Actions.Clear();
        _inputDevices.ForEach(x => _inputState.Actions.UnionWith(x.GetInputActions()));
    }

    /// <summary>
    /// Subscribes to have an action called when a button is pressed
    /// Triggers once per press
    /// </summary>
    /// <param name="actionType">The type of action to subscribe to</param>
    /// <param name="callback">The action to perform when the button is pressed</param>
    public void SubscribeOnPressed(InputAction actionType, Action callback) {
        if (_onPressedActions.ContainsKey(actionType)) {
            _onPressedActions[actionType].Add(callback);
        }
        else {
            _onPressedActions[actionType] = new HashSet<Action> { callback };
        }
    }

    /// <summary>
    /// Subscribes to have an action called while a button is pressed
    /// Triggers every frame a button is pressed
    /// </summary>
    /// <param name="actionType">The type of action to subscribe to</param>
    /// <param name="callback">The action to perform each frame</param>
    public void SubscribeWhilePressed(InputAction actionType, Action callback) {
        if (_whilePressedActions.ContainsKey(actionType)) {
            _whilePressedActions[actionType].Add(callback);
        }
        else {
            _whilePressedActions[actionType] = new HashSet<Action> { callback };
        }
    }

    public void Unsubscribe(InputAction actionType, Action callback) {
        _onPressedActions.GetValueOrDefault(actionType)?.Remove(callback);
    }

    public Vector2? GetAimDirection() {
        return _inputDevices.FirstOrDefault(x => x.Position != null)?.Position;
    }
    
    public bool TryGetDirectionalInput(out Vector2 direction) {
        // Vector2 direction = _gamepadState.ThumbSticks.Left;
        // direction.Y *= -1;	// invert the y-axis

        var hasInput = false;
        
        direction = Vector2.Zero;

        if (IsActionKeyDown(InputAction.MoveLeft)) {
            direction.X -= 1;
            hasInput = true;
        }
        if (IsActionKeyDown(InputAction.MoveRight)) {
            direction.X += 1;
            hasInput = true;
        }
        if (IsActionKeyDown(InputAction.MoveUp)) {
            direction.Y -= 1;
            hasInput = true;
        }
        if (IsActionKeyDown(InputAction.MoveDown)) {
            direction.Y += 1;
            hasInput = true;
        }
        
        direction = direction.ToNormalizedOrZero();

        return hasInput;
    }
    
    private void ProcessPlayerActions() {
        foreach (var heldAction in _inputState.Actions.Intersect(_lastInputState.Actions)) {
            foreach (var action in _whilePressedActions.GetValueOrDefault(heldAction) ?? Enumerable.Empty<Action>()) {
                action.Invoke();
            }
        }
        
        // foreach (var playerAction in _lastInputState.Actions.Except(_inputState.Actions)) {
        //     foreach (var action in _whilePressedActions.GetValueOrDefault(playerAction) ?? Enumerable.Empty<Action>()) {
        //         action.Invoke();
        //     }
        // }
        
        foreach (var pressedActions in _inputState.Actions.Except(_lastInputState.Actions)) {
            foreach (var action in _onPressedActions.GetValueOrDefault(pressedActions) ?? Enumerable.Empty<Action>()) {
                action.Invoke();
            }
        }
    }
    
    private record InputState(HashSet<InputAction> Actions);

}