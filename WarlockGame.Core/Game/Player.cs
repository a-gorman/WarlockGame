using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Entity.Order;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Spell;
using Warlock = WarlockGame.Core.Game.Entity.Warlock;

namespace WarlockGame.Core.Game;

class Player {
    public string Name { get; }
    
    public int Id { get; }

    public Entity.Warlock Warlock { get; }
    
    public bool IsActive { get; } = true;
    
    public Player(string name, int id, Entity.Warlock warlock) {
        Name = name;
        Id = id;
        Warlock = warlock;
    }

    public void Update() {
    }
}