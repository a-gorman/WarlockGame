using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;
using Color = Microsoft.Xna.Framework.Color;

namespace WarlockGame.Core.Game;

static class Configuration {
    public static string WindowName { get; set; } = null!;
    public static bool Server { get; set; }
    public static bool Client { get; set; }
    public static string JoinIp { get; set; } = null!;
    public static int ScreenWidth { get; set; }
    public static int ScreenHeight { get; set; }
    public static Dictionary<Keys, InputAction> KeyMappings { get; set; } = null!;
    public static bool RestartOnJoin { get; set; }
    
    // Player Settings
    public static string? PlayerName { get; set; }
    public static Color? PreferredColor { get; set; }
    public static Logger.Level LogDisplayLevel { get; set; }
    public static Logger.Level LogDedupeLevel { get; set; }
    public static bool LogDisplayVisible { get; set; }
    public static int EdgeScrollWidth { get; set; }
    public static float EdgeScrollSpeed { get; set; }
    public static float KeyScrollSpeed { get; set; }
    public static float MouseLookSensitivity { get; set; }
    public static int MapEdgeScrollLimitBoundary { get; set; }
    
    public static bool DebugBoundingBoxVisualize { get; set; }

    public static void ParseArgs(IConfigurationRoot args) {
        WindowName = args["windowName"] ?? "WarlockGame";
        Client = args["autoStartClient"]?.Let(bool.Parse) ?? false;
        Server = args["autoStartServer"]?.Let(bool.Parse) ?? false;
        RestartOnJoin = args["autoRestartOnJoin"]?.Let(bool.Parse) ?? false;
        JoinIp = args["joinIp"] ?? "localhost";
        ScreenHeight = args["screenHeight"]?.Let(int.Parse) ?? 1080;
        ScreenWidth = args["screenWidth"]?.Let(int.Parse) ?? 1920;
        PlayerName = args["player:name"];
        PreferredColor = args["player:color"]?.Let(s => System.Drawing.Color.FromName(s).Let(c => new Color(c.R, c.G, c.B, c.A)));
        MapEdgeScrollLimitBoundary = args["interface:mapEdgeScrollLimitBoundary"]?.Let(int.Parse) ?? 0;
        EdgeScrollWidth = args["interface:edgeScrollWidth"]?.Let(int.Parse) ?? 20;
        EdgeScrollSpeed = args["interface:edgeScrollSpeed"]?.Let(int.Parse) ?? 7;
        KeyScrollSpeed = args["interface:keyScrollSpeed"]?.Let(int.Parse) ?? 6;
        MouseLookSensitivity = args["interface:middleMouseLookSensitivity"]?.Let(int.Parse) ?? 1;
        KeyMappings = new Dictionary<Keys, InputAction> {
            { ParseKey(args["keymap:spell1"], Keys.Q), InputAction.Spell1 },
            { ParseKey(args["keymap:spell2"], Keys.W), InputAction.Spell2 },
            { ParseKey(args["keymap:spell3"], Keys.E), InputAction.Spell3 },
            { ParseKey(args["keymap:spell4"], Keys.R), InputAction.Spell4 },
            { ParseKey(args["keymap:spell5"], Keys.T), InputAction.Spell5 },
            { ParseKey(args["keymap:spell6"], Keys.D), InputAction.Spell6 },
            { ParseKey(args["keymap:spell7"], Keys.F), InputAction.Spell7 },
            { ParseKey(args["keymap:spell8"], Keys.G), InputAction.Spell8 },
            { ParseKey(args["keymap:spell9"], Keys.C), InputAction.Spell9 },
            { ParseKey(args["keymap:spell10"], Keys.V), InputAction.Spell10 },
            { ParseKey(args["keymap:moveUp"], Keys.Up), InputAction.MoveUp },
            { ParseKey(args["keymap:moveDown"], Keys.Down), InputAction.MoveDown },
            { ParseKey(args["keymap:moveLeft"], Keys.Left), InputAction.MoveLeft },
            { ParseKey(args["keymap:moveRight"], Keys.Right), InputAction.MoveRight },
            { ParseKey(args["keymap:exit"], Keys.Escape), InputAction.Exit },
            { ParseKey(args["keymap:pause"], Keys.P), InputAction.Pause },
            { ParseKey(args["keymap:openCommandInput"], Keys.Enter), InputAction.OpenCommandInput },
        };
        
        LogDisplayLevel = args["logDisplayLevel"]?.Let(x => Logger.Level.ParseOrNull(x, true)) ?? Logger.Level.ERROR;
        LogDisplayVisible = args["logDisplayVisible"]?.Let(bool.Parse) ?? true;
        LogDedupeLevel = args["logDedupeLevel"]?.Let(x => Logger.Level.ParseOrNull(x, true)) ?? Logger.Level.ERROR;
        
        DebugBoundingBoxVisualize = args["debug:boundingBoxVisualize"]?.Let(bool.Parse) ?? false;
    }

    private static Keys ParseKey(string? str, Keys defaultValue) {
        return Enum.TryParse(str, true, out Keys key) ? key : defaultValue;
    }
}