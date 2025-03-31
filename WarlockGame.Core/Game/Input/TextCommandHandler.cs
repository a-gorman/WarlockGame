using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Input;

class TextCommandHandler {
    private record TextCommand(string Name, string[] Aliases, string Description, Action<string[]> Handler);

    private static Dictionary<string, TextCommand> _textCommandHandlers = new();
    private static List<string> _textCommandNames = new();


    public void Initialize() {
        RegisterTextCommand("help", ["h"], args => Help());
        RegisterTextCommand("exit", ["quit", "q"], args => WarlockGame.Instance.Exit());
        RegisterTextCommand("restart", ["rm"], args => Restart());
        RegisterTextCommand("host", args => Host());
        RegisterTextCommand("join", args => Join());
        RegisterTextCommand("check", ["checksum"],
            args => Logger.Info($"Checksum is: {WarlockGame.Instance.Simulation?.CalculateChecksum() ?? 0}"));
        RegisterTextCommand("logs", args => Logs(args.FirstOrDefault()),
            "Args: on | off | debug | info | warn | error");
    }

    public void HandleCommand(string command) {
        var args = command.ToLowerInvariant().Split(' ');
        if (_textCommandHandlers.TryGetValue(args[0], out var commandHandler)) {
            commandHandler.Handler.Invoke(args.Skip(1).ToArray());
        }
        else {
            Logger.Info("Command not recognized");
        }
    }

    private void RegisterTextCommand(TextCommand textCommand) {
        _textCommandHandlers[textCommand.Name] = textCommand;
        foreach (var alias in textCommand.Aliases) {
            _textCommandHandlers[alias] = textCommand;
        }

        _textCommandNames.Add(textCommand.Name);
    }

    private void RegisterTextCommand(string name, Action<string[]> handler, string description = "") =>
        RegisterTextCommand(name, [], handler, description);

    private void RegisterTextCommand(string name, string[] aliases, Action<string[]> handler, string description = "") {
        var lowerCaseName = name.ToLowerInvariant();
        var lowerCaseAliases = aliases.Select(x => x.ToLowerInvariant()).ToArray();
        var textCommand = new TextCommand(lowerCaseName, lowerCaseAliases, description, handler);
        RegisterTextCommand(textCommand);
    }

    private static void Restart() {
        if (NetworkManager.IsClient) {
            Logger.Info("Must be server host to restart game");
            return;
        }

        if (WarlockGame.IsLocal) {
            WarlockGame.Instance.RestartGame(Random.Shared.Next());
        }
        else if (NetworkManager.IsServer) {
            CommandManager.AddServerCommand(new StartGame { Seed = Random.Shared.Next() });
        }
    }

    private static void Host() {
        if (NetworkManager.IsConnected) {
            Logger.Info("Already in game!");
            return;
        }

        UIManager.OpenTextPrompt("Enter name:", name => {
            PlayerManager.AddLocalPlayer(name);
            NetworkManager.StartServer();
        });
    }

    private static void Join() {
        if (NetworkManager.IsConnected) {
            Logger.Info("Already in game!");
            return;
        }

        UIManager.OpenTextPrompt("Enter name:",
            name => {
                UIManager.OpenTextPrompt("Enter Host IP Address:",
                    ipAddress => {
                        NetworkManager.ConnectToServer(ipAddress.NullOrEmptyTo("localhost"),
                            () => NetworkManager.JoinGame(name));
                    });
            });
    }

    private static void Help() {
        var sb = new StringBuilder("Available commands:");

        foreach (var textCommand in _textCommandNames.Select(n => _textCommandHandlers[n])) {
            sb.AppendLine();
            sb.Append(textCommand.Name);
            if (textCommand.Aliases.Any()) {
                sb.Append('(');
                sb.AppendJoin(" or ", textCommand.Aliases);
                sb.Append(')');
            }
            sb.Append(':');

            sb.Append(textCommand.Description);
        }

        Logger.Info(sb.ToString());
    }

    private static void Logs(string? arg) {
        if (arg == null) {
            LogDisplay.Instance.Visible = !LogDisplay.Instance.Visible;
            return;
        }

        switch (arg) {
            case "debug":
                LogDisplay.Instance.DisplayLevel = Logger.Level.DEBUG;
                break;
            case "info":
                LogDisplay.Instance.DisplayLevel = Logger.Level.INFO;
                break;
            case "warn" or "warning":
                LogDisplay.Instance.DisplayLevel = Logger.Level.WARNING;
                break;
            case "error":
                LogDisplay.Instance.DisplayLevel = Logger.Level.ERROR;
                break;
            case "on" or "visible":
                LogDisplay.Instance.Visible = true;
                break;
            case "off" or "hide":
                LogDisplay.Instance.Visible = false;
                break;
        }
    }
}