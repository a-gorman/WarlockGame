using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.Networking;

namespace NeonShooter.Core; 

static class InputManager {

    private static readonly Dictionary<int, List<MoveAction>> _moveActions = new();

    public static void Update(int frame) {
        if (_moveActions.TryGetValue(frame, out var actions)) {
            foreach (var action in actions) {
                PlayerManager.Players.First(x => x.Id == action.PlayerId)
                             .Warlock
                             .GiveOrder(x => new DestinationMoveOrder(action.Location ,x));
            }

            _moveActions.Remove(frame);
        }
    }

    public static void AddMoveAction(MoveAction moveAction) {
        if(!_moveActions.ContainsKey(moveAction.TargetFrame)) _moveActions.Add(moveAction.TargetFrame, new List<MoveAction>());
        _moveActions[moveAction.TargetFrame].Add(moveAction);
    }
}