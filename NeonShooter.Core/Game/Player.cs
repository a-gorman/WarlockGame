using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.UX;

namespace NeonShooter.Core.Game; 

class Player {
    public required string Name { get; init; }

    public PlayerInput Input { get; } = new();

    public PlayerStatus Status { get; }
    
    public PlayerShip Warlock { get; set; }

    private bool initialized;

    public Player() {
        Status = new(this);
    }

    public void Initialize() {
        if (initialized) {
            return;
        }
        
        SetupInputActions();
        initialized = true;
    }
    
    public void Update() {
        Status.Update();
        Input.Update();
    }

    private void SetupInputActions() {
        foreach (var inputAction in new[] {InputAction.MoveLeft, InputAction.MoveRight, InputAction.MoveUp, InputAction.MoveDown}) {
            Input.SubscribeOnPressed(inputAction, () => { Warlock.GiveOrder(x => new DirectionMoveOrder(x)); });
        }
    }
}

