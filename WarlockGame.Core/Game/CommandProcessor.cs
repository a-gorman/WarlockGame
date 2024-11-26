using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity.Factory;
using WarlockGame.Core.Game.Entity.Order;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game; 

static class CommandProcessor {

    private static readonly PriorityQueue<IServerCommand, int> _serverCommands = new();
    private static List<IServerCommand> _processedServerCommands = new();
    public static IReadOnlyList<IServerCommand> ProcessedServerCommands => _processedServerCommands;
    
    private static readonly PriorityQueue<IPlayerCommand, int> _playerCommands = new();
    private static List<IPlayerCommand> _processedPlayerCommands = new();
    public static IReadOnlyList<IPlayerCommand> ProcessedPlayerCommands => _processedPlayerCommands;
    
    public static void Update(int currentFrame) {
        _processedServerCommands.Clear();
        _processedPlayerCommands.Clear();
        
        while (_serverCommands.TryPeek(out var command, out var frame) && frame == currentFrame) {
            _processedServerCommands.Add(_serverCommands.Dequeue());
            IssueServerCommand(command);
        }
        
        while (_playerCommands.TryPeek(out var command, out var frame) && frame == currentFrame) {
            _processedPlayerCommands.Add(_playerCommands.Dequeue());
            
            if (PlayerManager.GetPlayer(command.PlayerId)?.IsActive ?? true) {
                IssuePlayerCommand(command);
            }
        }
    }

    private static void IssueServerCommand(IServerCommand command) {
        switch (command) {
            case StartGame sg:
                WarlockGame.Instance.RestartGame(sg.Seed);
                break;
            case PlayerRemoved rm:
                PlayerManager.RemovePlayer(rm.PlayerId);
                break;
        }
    }
    
    private static void IssuePlayerCommand(IPlayerCommand action) {
        switch (action) {
            case MoveCommand move:
                IssueMoveCommand(move.PlayerId, move.Location);
                break;
            case CastCommand cast:
                IssueCastCommand(cast.PlayerId, cast.CastVector, cast.SpellId);
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

    public static void AddDelayedServerCommand(IServerCommand command, int targetFrame) {
        _serverCommands.Enqueue(command, targetFrame);
    }
    
    private static void IssueMoveCommand(int playerId, Vector2 location) {
        EntityManager.GetWarlockByPlayerId(playerId)?.GiveOrder(x => new DestinationMoveOrder(location, x));
    }
    
    private static void IssueCastCommand(int playerId, Vector2 location, int spellId) {
        EntityManager.GetWarlockByPlayerId(playerId)?.GiveOrder(x => new CastOrder(spellId, location, x));
    }

    public static void Clear() {
        _serverCommands.Clear();
        _playerCommands.Clear();
    }
}