using System.Collections.Generic;
using WarlockGame.Core.Game.Networking.Packet;

namespace WarlockGame.Core.Game;

static class CommandManager {

    private static List<IServerCommand> _serverCommands = new();
    private static List<IServerCommand> _processedServerCommands = new();
    public static IReadOnlyList<IServerCommand> ProcessedServerCommands => _processedServerCommands;

    public static List<IPlayerAction> SimulationCommands { get; } = new();

    public static void Update() {
        (_serverCommands, _processedServerCommands) = (_processedServerCommands, _serverCommands);
        _serverCommands.Clear();
        foreach (var command in _processedServerCommands) {
            IssueServerCommand(command);
        }
    }

public static void IssueServerCommand(IServerCommand command) {
        switch (command) {
            case StartGame sg:
                WarlockGame.Instance.RestartGame(sg.Seed);
                break;
        }
    }

    /// <summary>
    /// Issues a player command to be executed on the next tick.
    /// You probably don't want to use this method
    /// </summary>
    public static void AddSimulationCommand(IPlayerAction action) {
        SimulationCommands.Add(action);
    }

    /// <summary>
    /// Issues a server command to be executed on the next tick.
    /// You probably don't want to use this method
    /// </summary>
    public static void AddServerCommand(IServerCommand command) {
        _serverCommands.Add(command);
    }

    public static void Clear() {
        _serverCommands.Clear();
        SimulationCommands.Clear();
    }
}