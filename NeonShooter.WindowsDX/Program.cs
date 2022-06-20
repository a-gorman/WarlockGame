using System;
using NeonShooter.Core;

namespace NeonShooter.WindowsDX
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new NeonShooterGame())
                game.Run();
        }
    }
}
