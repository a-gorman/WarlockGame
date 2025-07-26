using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Sim.Entities.Factory;

class WarlockFactory(Simulation simulation) {
    public Warlock CreateWarlock(int playerId, Vector2 position) {
        var warlock = new Warlock(playerId, position);
        warlock.Spells.AddRange([
            simulation.SpellFactory.Fireball(),
            simulation.SpellFactory.Lightning(),
            simulation.SpellFactory.Poison(),
            simulation.SpellFactory.Burst(),
            simulation.SpellFactory.SoulShatter(),
            simulation.SpellFactory.WindShield(),
            simulation.SpellFactory.RefractionShield(),
            simulation.SpellFactory.Homing()
        ]);

        warlock.Sprite.Color = PlayerManager.GetPlayer(playerId)!.Color;
        
        return warlock;
    }
}