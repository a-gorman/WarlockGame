using WarlockGame.Core.Game.Log;

namespace WarlockGame.Core.Game;

public static class Settings {
    public static bool Server { get; set; }
    public static bool Client { get; set; }
    public static string PlayerName { get; set; } = "default";
    public static string JoinIp { get; set; } = "localhost";

    public static class LogSettings {
        public static Logger.Level LogLevel { get; set; } = Logger.Level.INFO;
        public static bool Visible { get; set; }
    }
}