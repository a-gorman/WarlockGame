using System.Collections.Generic;

namespace WarlockGame.Core.Game.Sim;

class ForceManager {
    public Dictionary<int, Force> Forces { get; set; } = new();

    private int _nextForceId = 1;

    public void AddForce(Force force) {
        force.Id = _nextForceId++;
        Forces.Add(force.Id, force);
    }
}

// A team or side. The in-simulation variant of players, which represent real individuals or AIs.
// Hypothetically, two players could switch forces, for example after reloading a save.
class Force {
    public int Id { get; set; }
    
}