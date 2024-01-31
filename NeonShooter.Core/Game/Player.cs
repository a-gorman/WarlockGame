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

    public LocalPlayerInput Input { get; }

    public PlayerStatus Status { get; }

    // LATE INIT due to bidirectional reference. This will be set up by player factory
    public Warlock Warlock { get; set; } = null!;
    
    public WarlockSpell? SelectedSpell { get; private set; }

    private static readonly List<InputAction> SpellSelections = new() { InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4 };

    private bool _initialized;

    public Player(string name, int id, IEnumerable<IInputDevice> inputDevices) {
        Status = new(this);
        Input = new(inputDevices);
        Name = name;
        Id = id;
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
        var aimPosition = Input.GetAimPosition()!.Value;
        Warlock.GiveOrder(x => new DestinationMoveOrder(aimPosition, x));
        SelectedSpell = null;
        
        NetworkManager.SendPacket(new MoveAction {PlayerId = Id, TargetFrame = 0, Location = aimPosition});
    }
}

