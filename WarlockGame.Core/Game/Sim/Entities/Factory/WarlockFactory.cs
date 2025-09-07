namespace WarlockGame.Core.Game.Sim.Entities.Factory;

class WarlockFactory(Simulation simulation) {
    public Warlock CreateWarlock(int playerId, Vector2 position) {
        var warlock = new Warlock(playerId, position, simulation);

        warlock.Sprite.Color = PlayerManager.GetPlayer(playerId)!.Color;
        
        return warlock;
    }
}