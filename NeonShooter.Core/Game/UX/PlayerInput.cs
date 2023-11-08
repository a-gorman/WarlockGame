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

    private readonly Dictionary<InputAction, HashSet<Action<InputAction>>> _onPressedActions = new();
    private readonly Dictionary<InputAction, HashSet<Action<InputAction>>> _whilePressedActions = new();
    
    private InputState _inputState =  new();
    private InputState _lastInputState =  new();

    // Input devices to use for this player. For example Keyboard+Mouse or gamepad
    private readonly List<IInputDevice> _inputDevices = new();

    public PlayerInput(IEnumerable<IInputDevice> inputDevices) {
        _inputDevices.AddRange(inputDevices);
    }

    public bool IsActionKeyDown(InputAction action) => _inputState.Actions.Contains(action);
    
    public bool WasActionKeyPressed(InputAction action) => _inputState.Actions.Contains(action) && !_lastInputState.Actions.Contains(action);

    public bool WasDirectionalInputAdded() => _inputState.MovementDirection != null && _lastInputState.MovementDirection == null;
    
    public void Update() {
        CreateInputState();
        ProcessPlayerActions();
    }

    private void CreateInputState() {
        (_inputState, _lastInputState) = (_lastInputState, _inputState);
        
        _inputState.Actions.Clear();
        _inputDevices.ForEach(x => _inputState.Actions.UnionWith(x.GetInputActions()));
        _inputState.AimDirection = _inputDevices.FirstOrDefault(x => x.RightStick != null)?.RightStick;
        _inputState.MovementDirection = GetDirectionalInput();
    }

    /// <summary>
    /// Subscribes to have an action called when a button is pressed
    /// Triggers once per press
    /// </summary>
    /// <param name="actionType">The type of action to subscribe to</param>
    /// <param name="callback">The action to perform when the button is pressed</param>
    public void SubscribeOnPressed(InputAction actionType, Action<InputAction> callback) {
        if (_onPressedActions.ContainsKey(actionType)) {
            _onPressedActions[actionType].Add(callback);
        }
        else {
            _onPressedActions[actionType] = new HashSet<Action<InputAction>> { callback };
        }
    }

    /// <summary>
    /// Subscribes to have an action called while a button is pressed
    /// Triggers every frame a button is pressed
    /// </summary>
    /// <param name="actionType">The type of action to subscribe to</param>
    /// <param name="callback">The action to perform each frame</param>
    public void SubscribeWhilePressed(InputAction actionType, Action<InputAction> callback) {
        if (_whilePressedActions.ContainsKey(actionType)) {
            _whilePressedActions[actionType].Add(callback);
        }
        else {
            _whilePressedActions[actionType] = new HashSet<Action<InputAction>> { callback };
        }
    }

    // public void Unsubscribe(InputAction actionType, Action callback) {
    //     _onPressedActions.GetValueOrDefault(actionType)?.Remove(callback);
    // }

    public Vector2? GetAimDirection(Vector2 relativeTo) {
        var stickInput = _inputDevices.FirstOrDefault(x => x.RightStick != null)?.RightStick;
        if(stickInput != null) {
            return stickInput;
        }
        
        return (_inputDevices.FirstOrDefault(x => x.Position != null)?.Position - relativeTo)?.ToNormalizedOrZero();
    }
    
    public Vector2? GetAimPosition() {
        return _inputDevices.FirstOrDefault(x => x.Position != null)?.Position;
    }
    
    public Vector2? GetDirectionalInput() {
        // Vector2 direction = _gamepadState.ThumbSticks.Left;
        // direction.Y *= -1;	// invert the y-axis
        
        var gamepadDirection = _inputDevices.FirstOrDefault(x => x.LeftStick != null)?.LeftStick;
        if (gamepadDirection != null) {
            return gamepadDirection;
        }

        var hasInput = false;
        var direction = Vector2.Zero;

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

        return hasInput ? direction.ToNormalizedOrZero() : null;
    }
    
    private void ProcessPlayerActions() {
        foreach (var heldAction in _inputState.Actions.Intersect(_lastInputState.Actions)) {
            foreach (var callback in _whilePressedActions.GetValueOrDefault(heldAction) ?? Enumerable.Empty<Action<InputAction>>()) {
                callback.Invoke(heldAction);
            }
        }
        
        // foreach (var playerAction in _lastInputState.Actions.Except(_inputState.Actions)) {
        //     foreach (var action in _whilePressedActions.GetValueOrDefault(playerAction) ?? Enumerable.Empty<Action>()) {
        //         action.Invoke();
        //     }
        // }
        
        foreach (var pressedAction in _inputState.Actions.Except(_lastInputState.Actions)) {
            foreach (var callback in _onPressedActions.GetValueOrDefault(pressedAction) ?? Enumerable.Empty<Action<InputAction>>()) {
                callback.Invoke(pressedAction);
            }
        }
    }

    private record InputState {
        public HashSet<InputAction> Actions { get; } = new();
        public Vector2? AimDirection = null;
        public Vector2? CursorPosition = null;
        public Vector2? MovementDirection = null;
    }

}