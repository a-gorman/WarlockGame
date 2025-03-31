using System;

namespace WarlockGame.WindowsDX
{
    public static class Program
    {
        [STAThread]
        private static void Main() {
            using var game = new Core.WarlockGame();
            game.Run();
        }
    }
}
