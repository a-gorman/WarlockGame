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

    // Input devices to use for this player. For example Keyboard+Mouse or gamepad
    private readonly List<IInputDevice> _inputDevices = new() {new KeyboardInput()};

    public bool WasActionKeyPressed(InputAction action) => _inputDevices.Any(x => x.WasActionKeyPressed(action));

    public bool IsActionKeyDown(InputAction action) => _inputDevices.Any(x => x.IsActionKeyDown(action));

    public void Update() {
        foreach (var inputDevice in _inputDevices) {
            (inputDevice as KeyboardInput).Update();
        }

        ProcessPlayerActions();
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

    public Vector2 GetDirectionalInput() {
        // Vector2 direction = _gamepadState.ThumbSticks.Left;
        // direction.Y *= -1;	// invert the y-axis

        var direction = Vector2.Zero;
        
        if (IsActionKeyDown(InputAction.MoveLeft))
            direction.X -= 1;
        if (IsActionKeyDown(InputAction.MoveRight))
            direction.X += 1;
        if (IsActionKeyDown(InputAction.MoveUp))
            direction.Y -= 1;
        if (IsActionKeyDown(InputAction.MoveDown))
            direction.Y += 1;

        return direction.ToNormalizedOrZero();
    }
    
    public void ProcessPlayerActions() {
        foreach (var playerAction in _inputDevices.SelectMany(x => x.GetHeldActions())) {
            foreach (var action in _whilePressedActions.GetValueOrDefault(playerAction) ?? Enumerable.Empty<Action>()) {
                action.Invoke();
            }
        }
        
        // foreach (var playerAction in _inputDevices.SelectMany(x => x.GetReleasedActions())) {
        //     foreach (var action in _whilePressedActions.GetValueOrDefault(playerAction) ?? Enumerable.Empty<Action>()) {
        //         action.Invoke();
        //     }
        // }
        
        foreach (var playerAction in _inputDevices.SelectMany(x => x.GetPressedActions())) {
            foreach (var action in _onPressedActions.GetValueOrDefault(playerAction) ?? Enumerable.Empty<Action>()) {
                action.Invoke();
            }
        }
    }
}