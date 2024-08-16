using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity.Factory;
using WarlockGame.Core.Game.Entity.Order;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game; 

static class CommandProcessor {

    private static readonly PriorityQueue<IPlayerCommand, int> _playerCommands = new();
    private static readonly PriorityQueue<ISynchronizedCommand, int> _serverCommands = new();
    
    public static void Update(int currentFrame) {
        while (_serverCommands.TryPeek(out var command, out var frame) && frame == currentFrame) {
            _serverCommands.Dequeue();
            IssueServerCommand(command);
        }
        
        while (_playerCommands.TryPeek(out var command, out var frame) && frame == currentFrame) {
            _playerCommands.Dequeue();
            
            if (PlayerManager.GetPlayer(command.PlayerId)?.IsActive ?? true) {
                IssuePlayerCommand(command);
            }
        }
    }

    private static void IssueServerCommand(ISynchronizedCommand command) {
        switch (command) {
            case StartGame:
                WarlockGame.Instance.RestartGame();
                break;
        }
    }
    
    private static void IssuePlayerCommand(IPlayerCommand action) {
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
    /// <param name="command">The command to issue later</param>
    /// <param name="targetFrame">The frame to issue the command on</param>
    public static void AddDelayedPlayerCommand(IPlayerCommand command, int targetFrame) {
        _playerCommands.Enqueue(command, targetFrame);
    }

    public static void AddDelayedGameCommand(ISynchronizedCommand command) {
        _serverCommands.Enqueue(command, command.TargetFrame);
    }
    
    public static void IssueMoveCommand(int playerId, Vector2 location) {
        EntityManager.GetWarlockByPlayerId(playerId)?.GiveOrder(x => new DestinationMoveOrder(location, x));
    }
    
    public static void IssueCastCommand(int playerId, Vector2 location, int spellId) {
        EntityManager.GetWarlockByPlayerId(playerId)?.GiveOrder(x => new CastOrder(spellId, location, x));
    }

    public static void Clear() {
        _serverCommands.Clear();
        _playerCommands.Clear();
    }
}