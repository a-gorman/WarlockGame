using System;
using WarlockGame.Core.Game.Log;

namespace WarlockGame.DesktopGL
{
    public static class Program
    {
        [STAThread]
        static void Main(params string[] args) {
            try {
                using var game = new WarlockGame.Core.WarlockGame(args);
                game.Run();
            } catch (Exception e) {
                Logger.WriteLog(e.ToString(), Logger.Level.FATAL, Logger.LogType.Program);
                Logger.WriteLogsToFile("crashdump");
                throw;
            }
        }
    }
}
