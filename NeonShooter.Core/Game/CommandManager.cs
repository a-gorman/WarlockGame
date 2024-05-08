using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.Networking;

namespace NeonShooter.Core.Game; 

static class CommandManager {

    private static readonly Dictionary<int, List<IGameCommand>> _commands = new();
    
    public static void Update(int frame) {
        if (_commands.TryGetValue(frame, out var actions)) {
            foreach (var action in actions) {
                // Prevent disconnected players from have buffered pause commands and such
                if (!PlayerManager.GetPlayer(action.PlayerId)?.IsActive ?? false) {
                    continue;
                }

                IssueGameCommand(action);
            }
        }
    }

    private static void IssueGameCommand(IGameCommand action) {
        switch (action) {
            case MoveCommand move:
                IssueMoveCommand(move.PlayerId, move.Location);
                return;
            case CastCommand cast:
                IssueCastCommand(cast.PlayerId, cast.Location, cast.SpellId);
                return;
        }
    }

    /// <summary>
    /// Issues a command to be executed on a future frame. Necessary to handle network delay
    /// </summary>
    /// <param name="moveCommand">The command to issue later</param>
    /// <param name="targetFrame">The frame to issue the command on</param>
    public static void AddDelayedGameCommand(IGameCommand moveCommand, int targetFrame) {
        if(!_commands.ContainsKey(targetFrame)) _commands.Add(targetFrame, new List<IGameCommand>());
        _commands[targetFrame].Add(moveCommand);
    }

    public static void IssueMoveCommand(int playerId, Vector2 location) {
        PlayerManager.Players.First(x => x.Id == playerId)
                     .Warlock
                     .GiveOrder(x => new DestinationMoveOrder(location, x));
    }
    
    public static void IssueCastCommand(int playerId, Vector2 location, int spellId) {
        PlayerManager.Players.First(x => x.Id == playerId)
                     .Warlock
                     .GiveOrder(x => new CastOrder(spellId, location, x));
    }
}