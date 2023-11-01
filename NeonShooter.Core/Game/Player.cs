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
    }

    private void SetupInputActions() {
        Input.SubscribeWhilePressed(InputAction.MoveLeft,
            () => { Warlock.GiveOrder(x => new DirectionMoveOrder(new Vector2(-1, 0), x)); });
        Input.SubscribeWhilePressed(InputAction.MoveRight,
            () => { Warlock.GiveOrder(x => new DirectionMoveOrder(new Vector2(1, 0), x)); });
        Input.SubscribeWhilePressed(InputAction.MoveUp,
            () => { Warlock.GiveOrder(x => new DirectionMoveOrder(new Vector2(0, -1), x)); });
        Input.SubscribeWhilePressed(InputAction.MoveDown,
            () => { Warlock.GiveOrder(x => new DirectionMoveOrder(new Vector2(0, 1), x)); });
    }
    
}

