using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
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
        RegisterTextCommand("help", ["h"], _ => Help());
        RegisterTextCommand("exit", ["quit", "q"], _ => WarlockGame.Instance.Exit());
        RegisterTextCommand("restart", ["rm"], _ => Restart());
        RegisterTextCommand("host", _ => Host());
        RegisterTextCommand("join", _ => Join());
        RegisterTextCommand("check", ["checksum"],
            _ => MessageDisplay.AddMessage($"Checksum is: {WarlockGame.Instance.Simulation?.CalculateChecksum() ?? 0}"));
        RegisterTextCommand("logs", Logs, "Args: on | off | debug | info | warn | error");
        RegisterTextCommand("ip", 
            _ => MessageDisplay.AddMessage($"IP Address is: {NetUtils.GetLocalIpList(LocalAddrType.IPv4).JoinToString()}"));
    }

    public void HandleCommand(string command) {
        var args = command.ToLowerInvariant().Split(' ');
        if (_textCommandHandlers.TryGetValue(args[0], out var commandHandler)) {
            commandHandler.Handler.Invoke(args.Skip(1).ToArray());
        }
        else {
            MessageDisplay.AddMessage("Command not recognized");
        }
    }

    private void RegisterTextCommand(string name, Action<string[]> handler, string description = "") =>
        RegisterTextCommand(name, [], handler, description);

    private void RegisterTextCommand(string name, string[] aliases, Action<string[]> handler, string description = "") {
        var lowerCaseName = name.ToLowerInvariant();
        var lowerCaseAliases = aliases.Select(x => x.ToLowerInvariant()).ToArray();
        var textCommand = new TextCommand(lowerCaseName, lowerCaseAliases, description, handler);
        RegisterTextCommand(textCommand);
    }
    
    private void RegisterTextCommand(TextCommand textCommand) {
        _textCommandHandlers[textCommand.Name] = textCommand;
        foreach (var alias in textCommand.Aliases) {
            _textCommandHandlers[alias] = textCommand;
        }

        _textCommandNames.Add(textCommand.Name);
    }

    private static void Restart() {
        if (NetworkManager.IsClient) {
            MessageDisplay.AddMessage("Must be server host to restart game");
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
            MessageDisplay.AddMessage("Already in game!");
            return;
        }

        UIManager.OpenTextPrompt("Enter name:", name => {
            PlayerManager.AddLocalPlayer(name);
            NetworkManager.StartServer();
        });
    }

    private static void Join() {
        if (NetworkManager.IsConnected) {
            MessageDisplay.AddMessage("Already in game!");
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

        MessageDisplay.AddMessage(sb.ToString());
    }

    private static void Logs(string[] args) {
        switch (args.ElementAtOrDefault(0)?.ToLowerInvariant()) {
            case "level":
                switch (args.ElementAtOrDefault(1)?.ToLowerInvariant())
                {
                    case "debug":
                        LogDisplay.Instance.DisplayLevel = Logger.Level.DEBUG;
                        return;
                    case "info":
                        LogDisplay.Instance.DisplayLevel = Logger.Level.INFO;
                        return;
                    case "warn" or "warning":
                        LogDisplay.Instance.DisplayLevel = Logger.Level.WARNING;
                        return;
                    case "error":
                        LogDisplay.Instance.DisplayLevel = Logger.Level.ERROR;
                        return;
                    case null:
                        return;
                }
                return;
            case "dedupe":
                switch (args.ElementAtOrDefault(1)?.ToLowerInvariant())
                {
                    case "debug" or "0":
                        Logger.DedupeLevel = Logger.Level.DEBUG;
                        return;
                    case "info" or "1":
                        Logger.DedupeLevel = Logger.Level.INFO;
                        return;
                    case "warning" or "2":
                        Logger.DedupeLevel = Logger.Level.WARNING;
                        return;
                    case "error" or "3":
                        Logger.DedupeLevel = Logger.Level.NONE;
                        return;
                    case null or "off" or "-1":
                        Logger.DedupeLevel = Logger.Level.NONE;
                        return;
                }
                return;
            case "on" or "visible":
                LogDisplay.Instance.Visible = true;
                return;
            case "off" or "hide":
                LogDisplay.Instance.Visible = false;
                return;
            case null:
                LogDisplay.Instance.Visible = !LogDisplay.Instance.Visible;
                return;
        }
    }
}