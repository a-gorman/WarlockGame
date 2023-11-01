using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.Core.Game.UX; 

/// <summary>
/// Handles input from a particular player
/// </summary>
class PlayerInput {

    private readonly Dictionary<InputAction, HashSet<Action>> _onPressedActions = new();
    private readonly Dictionary<InputAction, HashSet<Action>> _whilePressedActions = new();

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

    public void InputPlayerActions(IEnumerable<(InputAction actionType, bool wasPressed)> playerActions) {
        foreach (var playerAction in playerActions) {
            foreach (var action in _whilePressedActions.GetValueOrDefault(playerAction.actionType) ?? Enumerable.Empty<Action>()) {
                action.Invoke();
            }

            if (playerAction.wasPressed) {
                foreach (var action in _onPressedActions.GetValueOrDefault(playerAction.actionType) ?? Enumerable.Empty<Action>()) {
                    action.Invoke();
                }
            }
        }
    }
}