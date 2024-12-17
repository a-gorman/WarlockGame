using System.Collections.Generic;
using WarlockGame.Core.Game.Networking.Packet;

namespace WarlockGame.Core.Game; 

static class CommandManager {

    private static readonly List<IServerCommand> _serverCommands = new();
    private static readonly List<IServerCommand> _processedServerCommands = new();
    public static IReadOnlyList<IServerCommand> ProcessedServerCommands => _processedServerCommands;
    
    private static readonly PriorityQueue<IPlayerCommand, int> _simulationCommands = new();
    private static readonly List<IPlayerCommand> _currentSimulationCommands = new();
    public static IReadOnlyList<IPlayerCommand> CurrentSimulationCommands => _currentSimulationCommands;
    
    public static void Update(int currentTick) {
        _processedServerCommands.Clear();
        _currentSimulationCommands.Clear();

        foreach (var command in _serverCommands) {
            _processedServerCommands.Add(command);
            IssueServerCommand(command);
        }

        _serverCommands.Clear();
        
        while (_simulationCommands.TryPeek(out _, out var tick) && tick == currentTick) {
            _currentSimulationCommands.Add(_simulationCommands.Dequeue());
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

    /// <summary>
    /// Issues a command to be executed on a future frame.
    /// </summary>
    /// <param name="command">The command to issue later</param>
    /// <param name="targetFrame">The frame to issue the command on</param>
    public static void AddDelayedPlayerCommand(IPlayerCommand command, int targetFrame) {
        _simulationCommands.Enqueue(command, targetFrame);
    }

    public static void AddServerCommand(IServerCommand command) {
        _serverCommands.Add(command);
    }

    public static void Clear() {
        _serverCommands.Clear();
        _simulationCommands.Clear();
    }
    
    public static void ClearSimulationCommands() {
        _simulationCommands.Clear();
    }
}