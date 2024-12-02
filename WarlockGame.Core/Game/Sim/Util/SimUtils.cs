using System.Linq;

namespace WarlockGame.Core.Game.Sim.Util;

static class SimUtils {
    public static int CalculateChecksum() {
        return (int)EntityManager.Warlocks.Sum(x => x.Position.X + x.Position.Y);
    }
}