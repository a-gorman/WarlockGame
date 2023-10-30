using System.Linq;

namespace NeonShooter.Core.Game; 

static class GameStatus {
    public static bool IsGameOver => PlayerManager.Players.First().Status.Lives == 0;
}