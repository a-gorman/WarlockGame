using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.Networking;
using NeonShooter.Core.Game.Spell;
using NeonShooter.Core.Game.UX;
using NeonShooter.Core.Game.UX.InputDevices;
using Warlock = NeonShooter.Core.Game.Entity.Warlock;

namespace NeonShooter.Core.Game;

class Player {
    public string Name { get; }
    
    public int Id { get; }

    public Warlock Warlock { get; }
    
    public bool IsActive { get; } = true;
    
    public Player(string name, int id, Warlock warlock) {
        Name = name;
        Id = id;
        Warlock = warlock;
    }

    public void Update() {
    }
}