using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity.Factory;
using WarlockGame.Core.Game.Entity.Order;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game; 

static class CommandProcessor {

    private static readonly Dictionary<int, List<IGameCommand>> _commands = new();
    
    public static void Update(int frame) {
        if (_commands.TryGetValue(frame, out var actions)) {
            foreach (var action in actions) {
                // Prevent disconnected players from have buffered pause commands and such
                // This might cause desyncs if disconnections are not themselves synchronized
                if (!PlayerManager.GetPlayer(action.PlayerId)?.IsActive ?? false) {
                    continue;
                }

                IssueGameCommand(action);
            }

            _commands.Remove(frame);
        }
    }

    private static void IssueGameCommand(IGameCommand action) {
        switch (action) {
            case MoveCommand move:
                IssueMoveCommand(move.PlayerId, move.Location);
                break;
            case CastCommand cast:
                IssueCastCommand(cast.PlayerId, cast.Location, cast.SpellId);
                break;
            case CreateWarlock create:
                EntityManager.Add(create.Warlock.Let(WarlockFactory.FromPacket));
                break;
        }
    }

    /// <summary>
    /// Issues a command to be executed on a future frame. Necessary to handle network delay
    /// </summary>
    /// <param name="moveCommand">The command to issue later</param>
    /// <param name="targetFrame">The frame to issue the command on</param>
    public static void AddDelayedGameCommand(IGameCommand moveCommand, int targetFrame) {
        if (!_commands.ContainsKey(targetFrame)) {
            _commands.Add(targetFrame, new List<IGameCommand>());
        }
        _commands[targetFrame].Add(moveCommand);
    }

    public static void IssueMoveCommand(int playerId, Vector2 location) {
        EntityManager.GetWarlockByPlayerId(playerId)?.GiveOrder(x => new DestinationMoveOrder(location, x));
    }
    
    public static void IssueCastCommand(int playerId, Vector2 location, int spellId) {
        EntityManager.GetWarlockByPlayerId(playerId)?.GiveOrder(x => new CastOrder(spellId, location, x));
    }

    public static void Clear() {
        _commands.Clear();
    }
}