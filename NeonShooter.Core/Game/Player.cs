using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.Spell;
using NeonShooter.Core.Game.UX;
using NeonShooter.Core.Game.UX.InputDevices;

namespace NeonShooter.Core.Game; 

class Player {
    public required string Name { get; init; }

    public PlayerInput Input { get; }

    public PlayerStatus Status { get; }

    public PlayerShip Warlock { get; set; } = null!;
    
    public WarlockSpell? SelectedSpell { get; private set; }

    private static readonly List<InputAction> SpellSelections = new() { InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4 };

    private bool _initialized;

    public Player(IEnumerable<IInputDevice> inputDevices) {
        Status = new(this);
        Input = new(inputDevices);
    }

    public void Initialize() {
        if (_initialized) {
            return;
        }
        
        SetupInputActions();
        _initialized = true;
    }
    
    public void Update() {
        Status.Update();
        Input.Update();

        if (Input.WasDirectionalInputAdded()) {
            Warlock.GiveOrder(x => new DirectionMoveOrder(x));
        }
    }

    private void SetupInputActions() {
        // foreach (var inputAction in new[] { InputAction.MoveLeft, InputAction.MoveRight, InputAction.MoveUp, InputAction.MoveDown }) {
        //     Input.SubscribeOnPressed(inputAction, (_) => );
        // }
        
        foreach (var inputAction in SpellSelections) {
            Input.SubscribeOnPressed(inputAction, OnSpellSelected);
        }
        
        Input.SubscribeOnPressed(InputAction.Select, OnSelect);
        Input.SubscribeOnPressed(InputAction.RightClick, OnRightClick);
    }

    private void OnSpellSelected(InputAction inputAction) {
        SelectedSpell = Warlock.Spells.ElementAtOrDefault(SpellSelections.IndexOf(inputAction));
    }

    private void OnSelect(InputAction inputAction) {
        var inputDirection = Input.GetAimDirection(Warlock.Position);
        if (inputDirection != null && SelectedSpell != null) {
            Warlock.GiveOrder(x => new CastOrder(SelectedSpell, inputDirection.Value, x));
        }
        SelectedSpell = null;
    }

    private void OnRightClick(InputAction inputAction) {
        Warlock.GiveOrder(x => new DestinationMoveOrder(Input.GetAimPosition()!.Value, x));
        SelectedSpell = null;
    }
}
