using System.Collections.Generic;
using WarlockGame.Core.Game.Networking.Packet;

namespace WarlockGame.Core.Game;

// The way this class works is a mess. I never have a good idea of how it works.
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

    // Immediately processes the server command. Don't add extra call sites to this method.
    public static void IssueServerCommand(IServerCommand command) {
        switch (command) {
            case StartGame sg:
                WarlockGame.Instance.RestartGame(sg.Seed);
                break;
            case PlayerRemoved pr:
                PlayerManager.GetPlayer(pr.PlayerId)?.IsActive = false;
                break;
        }
    }

    /// <summary>
    /// Issues a player command to be executed on the next tick.
    /// Danger: You probably don't want to use this method if you are (or could be) the client.
    /// Instead, use the input manager's HandlePlayerAction().
    /// </summary>
    public static void AddSimulationCommand(IPlayerAction action) {
        SimulationCommands.Add(action);
    }

    /// <summary>
    /// Issues a server command to be executed on the next tick.
    /// Danger: You probably don't want to use this method if you are (or could be) the client.
    /// </summary>
    public static void AddServerCommand(IServerCommand command) {
        _serverCommands.Add(command);
    }

    public static void Clear() {
        _serverCommands.Clear();
        SimulationCommands.Clear();
    }
    
    public static void OnGameRestart() {
        _serverCommands.Clear();
    }
}