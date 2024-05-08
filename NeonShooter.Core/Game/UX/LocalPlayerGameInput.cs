using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.Networking;
using NeonShooter.Core.Game.Util;
using NeonShooter.Core.Game.UX.InputDevices;

namespace NeonShooter.Core.Game.UX; 

/// <summary>
/// Handles game input from a particular player
/// </summary>
class LocalPlayerGameInput {

    private InputState _inputState = new();
    private InputState _lastInputState = new();

    // Input devices to use for this player. For example Keyboard+Mouse or gamepad
    private readonly List<IInputDevice> _inputDevices = new();

    private static readonly List<InputAction> SpellSelectionActions = new() { InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4 };

    private int? SelectedSpellId { get; set; }

    private readonly Player _player;
    
    public LocalPlayerGameInput(IEnumerable<IInputDevice> inputDevices, Player player) {
        _inputDevices.AddRange(inputDevices);
        _player = player;
    }

    public bool IsActionKeyDown(InputAction action) => _inputState.Actions.Contains(action);
    
    public bool WasActionKeyPressed(InputAction action) => _inputState.Actions.Contains(action) && !_lastInputState.Actions.Contains(action);

    public bool WasDirectionalInputAdded() => _inputState.MovementDirection != null && _lastInputState.MovementDirection == null;

    public void Update() {
        if (WarlockGame.Instance.IsActive) {
            CreateInputState();
            ProcessPlayerActions();
        }
        else {
            _inputState.Clear();
            _lastInputState.Clear();
        }
    }

    private void CreateInputState() {
        (_inputState, _lastInputState) = (_lastInputState, _inputState);
        
        _inputState.Actions.Clear();
        _inputDevices.ForEach(x => _inputState.Actions.UnionWith(x.GetInputActions()));
        _inputState.MovementDirection = GetDirectionalInput();
    }

    public Vector2? GetAimDirection(Vector2 relativeTo) {
        return (_inputDevices.FirstOrDefault(x => x.Position != null)?.Position - relativeTo)?.ToNormalizedOrZero();
    }
    
    public Vector2? GetAimPosition() {
        return _inputDevices.FirstOrDefault(x => x.Position != null)?.Position;
    }
    
    public Vector2? GetDirectionalInput() {
        // Vector2 direction = _gamepadState.ThumbSticks.Left;
        // direction.Y *= -1;	// invert the y-axis
        
        var hasInput = false;
        var direction = Vector2.Zero;

        if (IsActionKeyDown(InputAction.MoveLeft)) {
            direction.X -= 1;
            hasInput = true;
        }
        if (IsActionKeyDown(InputAction.MoveRight)) {
            direction.X += 1;
            hasInput = true;
        }
        if (IsActionKeyDown(InputAction.MoveUp)) {
            direction.Y -= 1;
            hasInput = true;
        }
        if (IsActionKeyDown(InputAction.MoveDown)) {
            direction.Y += 1;
            hasInput = true;
        }

        return hasInput ? direction.ToNormalizedOrZero() : null;
    }
    
    private void ProcessPlayerActions() {
        foreach (var actionType in SpellSelectionActions) {
            if (WasActionKeyPressed(actionType)) {
                SelectedSpellId = _player.Warlock.Spells.ElementAtOrDefault(SpellSelectionActions.IndexOf(actionType))?.SpellId;
            }
        }
        
        if(WasActionKeyPressed(InputAction.Select)) OnSelect();
        if(WasActionKeyPressed(InputAction.RightClick)) OnRightClick();
        
        if (WasDirectionalInputAdded()) {
            _player.Warlock.GiveOrder(x => new DirectionMoveOrder(x));
        }
    }

    private void OnSelect() {
        var inputDirection = GetAimDirection(_player.Warlock.Position);
        if (inputDirection != null && SelectedSpellId != null) {
            if (WarlockGame.IsLocal) {
                CommandManager.IssueCastCommand(_player.Id, inputDirection.Value, SelectedSpellId.Value);
            }
            else {
                var moveAction = new CastCommand { PlayerId = _player.Id, Location = inputDirection.Value, SpellId = SelectedSpellId.Value};
                NetworkManager.SendPlayerCommand(moveAction);
                if(NetworkManager.IsServer) {
                    CommandManager.AddDelayedGameCommand(moveAction, WarlockGame.Frame + NetworkManager.FrameDelay);
                }
            }
        }
    }

    private void OnRightClick() {
        var aimPosition = GetAimPosition()!.Value;
        if (WarlockGame.IsLocal) {
            CommandManager.IssueMoveCommand(_player.Id, aimPosition);
        }
        else {
            var moveAction = new MoveCommand { PlayerId = _player.Id, Location = aimPosition };
            NetworkManager.SendPlayerCommand(moveAction);
            if(NetworkManager.IsServer) {
                CommandManager.AddDelayedGameCommand(moveAction, WarlockGame.Frame + NetworkManager.FrameDelay);
            }
        }
        SelectedSpellId = null;
    }

    private record InputState {
        public HashSet<InputAction> Actions { get; } = new();
        public Vector2? MovementDirection = null;

        public void Clear() {
            Actions.Clear();
            MovementDirection = null;
        }
    }

}