using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.UI.Components;
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
            _ => MessageDisplay.Display($"Checksum is: {WarlockGame.Instance.Simulation?.CalculateChecksum() ?? 0}"));
        RegisterTextCommand("logs", ["log"], Logs, "Args: on | off | debug | info | warn | error");
        RegisterTextCommand("ip", 
            _ => MessageDisplay.Display($"IP Address is: {NetUtils.GetLocalIpList(LocalAddrType.IPv4).JoinToString()}"));
    }

    public void HandleCommand(string command) {
        var args = command.ToLowerInvariant().Split(' ');
        if (_textCommandHandlers.TryGetValue(args[0], out var commandHandler)) {
            commandHandler.Handler.Invoke(args.Skip(1).ToArray());
        }
        else {
            MessageDisplay.Display("Command not recognized");
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
            MessageDisplay.Display("Must be server host to restart game");
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
            MessageDisplay.Display("Already in game!");
            return;
        }

        UIManager.OpenTextPrompt("Enter name:", name => {
            PlayerManager.AddLocalPlayer(name, Configuration.PreferredColor);
            NetworkManager.StartServer();
        });
    }

    private static void Join() {
        if (NetworkManager.IsConnected) {
            MessageDisplay.Display("Already in game!");
            return;
        }

        UIManager.OpenTextPrompt("Enter name:",
            name => {
                UIManager.OpenTextPrompt("Enter Host IP Address:",
                    ipAddress => {
                        NetworkManager.ConnectToServer(ipAddress.NullOrEmptyTo("localhost"),
                            () => NetworkManager.JoinGame(name, Configuration.PreferredColor));
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

        MessageDisplay.Display(sb.ToString());
    }

    private static void Logs(string[] args) {
        switch (args.ElementAtOrDefault(0)?.ToLowerInvariant()) {
            case "level":
                if (args.Length == 2) {
                    var level = GetLogLevel(args.ElementAt(1));
                    if (level.HasValue) {
                        LogDisplay.Instance.SetDisplayLevel(level.Value);
                    }
                } else if (args.Length > 2) {
                    var level = GetLogLevel(args.ElementAt(1));
                    if (level.HasValue) {
                        foreach (var typeString in args.Skip(2)) {
                            if (int.TryParse(typeString, out var typeInt)) {
                                LogDisplay.Instance.SetDisplayLevelForTypes((Logger.LogType)typeInt, level.Value);
                            }

                            Logger.LogType? type = typeString.ToLowerInvariant() switch {
                                "playeraction" or "player" => Logger.LogType.PlayerAction,
                                "interface" or "ui" => Logger.LogType.Interface,
                                "network" => Logger.LogType.Network,
                                "simulation" or "sim" => Logger.LogType.Simulation,
                                "program" => Logger.LogType.Program,
                                _ => null
                            };
                            if (type.HasValue) {
                                LogDisplay.Instance.SetDisplayLevelForTypes(type.Value, level.Value);
                            }
                        }
                    }
                }
                return;
            case "dedupe":
                if (args.Length == 2) {
                    var level = GetLogLevel(args.ElementAt(1));
                    if (level.HasValue) {
                        Logger.DedupeLevel = level.Value;
                    }
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

    private static Logger.Level? GetLogLevel(string input) {
        switch (input.ToLowerInvariant())
        {
            case "debug" or "0":
                return Logger.Level.DEBUG;
            case "info" or "1":
                return Logger.Level.INFO;
            case "warning" or "warn" or "2":
                return Logger.Level.WARNING;
            case "error" or "3":
                return Logger.Level.ERROR;
            case "off" or "-1":
                return Logger.Level.NONE;
            default:
                return null;
        }
    }
}