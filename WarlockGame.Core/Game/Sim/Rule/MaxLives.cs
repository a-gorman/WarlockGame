namespace WarlockGame.Core.Game.Sim.Rule;

public class PlayerStatus(int lives) {
    public int Lives = lives;
    public bool ChoosingPerk = false;
}

public struct LivesChanged {
    public bool Reset { get; set; }
    public int PlayerId { get; set; }
}