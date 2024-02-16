using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.Networking;

namespace NeonShooter.Core.Game; 

static class InputManager {

    private static readonly Dictionary<int, List<MoveAction>> _moveActions = new();
    
    public static void Update(int frame) {
        if (_moveActions.TryGetValue(frame, out var actions)) {
            foreach (var action in actions) {
                IssueMoveCommand(action.PlayerId, action.Location);
            }

            _moveActions.Remove(frame);
        }
    }

    public static void AddMoveAction(MoveAction moveAction, int targetFrame) {
        if(!_moveActions.ContainsKey(targetFrame)) _moveActions.Add(targetFrame, new List<MoveAction>());
        _moveActions[targetFrame].Add(moveAction);
    }

    public static void InputMoveAction(int playerId, Vector2 location) {
        if (WarlockGame.IsLocal) {
            IssueMoveCommand(playerId, location);
        }
        else {
            var moveAction = new MoveAction { PlayerId = playerId, Location = location };
            NetworkManager.SendPlayerCommand(moveAction);
            if(NetworkManager.IsServer) {
                AddMoveAction(moveAction, WarlockGame.Frame + NetworkManager.FrameDelay);
            }
        }
    }
    
    private static void IssueMoveCommand(int playerId, Vector2 location) {
        PlayerManager.Players.First(x => x.Id == playerId)
                     .Warlock
                     .GiveOrder(x => new DestinationMoveOrder(location, x));
    }
}