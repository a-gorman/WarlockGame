using System;

namespace WarlockGame.WindowsDX
{
    public static class Program
    {
        [STAThread]
        private static void Main(params string[] args) {
            using var game = new Core.WarlockGame(args);
            game.Run();
        }
    }
}
