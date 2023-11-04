using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.Spell;
using NeonShooter.Core.Game.UX;

namespace NeonShooter.Core.Game; 

class Player {
    public required string Name { get; init; }

    public PlayerInput Input { get; } = new();

    public PlayerStatus Status { get; }

    public PlayerShip Warlock { get; set; } = null!;
    
    public WarlockSpell? SelectedSpell { get; private set; }

    private static readonly List<InputAction> SpellSelections = new() { InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4 };

    private bool _initialized;

    public Player() {
        Status = new(this);
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
    }

    private void SetupInputActions() {
        foreach (var inputAction in new[] { InputAction.MoveLeft, InputAction.MoveRight, InputAction.MoveUp, InputAction.MoveDown }) {
            Input.SubscribeOnPressed(inputAction, (_) => Warlock.GiveOrder(x => new DirectionMoveOrder(x)));
        }
        
        foreach (var inputAction in SpellSelections) {
            Input.SubscribeOnPressed(inputAction, OnSpellSelected);
        }
        
        Input.SubscribeOnPressed(InputAction.Select, OnSelect);
        Input.SubscribeOnPressed(InputAction.RightClick, (_) => Warlock.GiveOrder(x => new DestinationMoveOrder(Input.GetAimPosition()!.Value, x)));
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

    // public static void HandleSpellButtonPressed()
    // {
    //     switch (InputType) {
    //         case InputType.MouseMove:
    //         case InputType.KeyboardMove:
    //
    //             for (var i = 0; i < _inputActions.Length; i++) {
    //                 if (StaticKeyboardInput.WasActionKeyPressed(_inputActions[i])) {
    //                     ActiveSpellIndex = i;
    //                     break;
    //                 }
    //             }
    //
    //             break;
    //         case InputType.Gamepad:
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException();
    //     }
    // }
    
    // if (ActiveSpellIndex != -1 && InputType != InputType.Gamepad && WasLeftMousePressed()) {
    //     PlayerManager.Players.First().Warlock.CastSpell(ActiveSpellIndex); // Assume the left mouse button is always player 1
    //     ActiveSpellIndex = -1;
    // }
}

