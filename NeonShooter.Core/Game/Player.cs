using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.UX;

namespace NeonShooter.Core.Game; 

class Player {
    public required string Name { get; init; }

    public Input Input { get; } = new();

    public PlayerStatus Status { get; }
    
    public PlayerShip Warlock { get; set; }

    public Player() {
        Status = new(this);
    }
        
    
    public void Update() {
        Status.Update();
    }
}

