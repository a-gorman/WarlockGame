using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Entity.Order;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;
using Warlock = WarlockGame.Core.Game.Entity.Warlock;

namespace WarlockGame.Core.Game.Input; 

/// <summary>
/// Handles game input from a particular player
/// </summary>
class LocalPlayerGameInput {
    private static readonly List<InputAction> SpellSelectionActions = new() { InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4 };

    private int? SelectedSpellId { get; set; }

    private readonly Player _player;

    public LocalPlayerGameInput(Player player) {
        _player = player;
    }
    
    public void Update(InputManager.InputState inputState) {
        var warlock = EntityManager.GetWarlockByPlayerId(_player.Id);
        if (warlock is null) return;
        
        if (!InputManager.HasTextConsumers) {
            foreach (var actionType in SpellSelectionActions) {
                if (inputState.WasActionKeyPressed(actionType)) {
                    SelectedSpellId = warlock.Spells.ElementAtOrDefault(SpellSelectionActions.IndexOf(actionType))?.SpellId;
                }
            }
        }
        
        if(inputState.WasActionKeyPressed(InputAction.LeftClick)) OnLeftClick(inputState, warlock.Position);
        if(inputState.WasActionKeyPressed(InputAction.RightClick)) OnRightClick(inputState);
    }

    // TODO: Find a way to dedup this logic
    private void OnLeftClick(InputManager.InputState inputState, Vector2 warlockPosition) {
        var inputDirection = inputState.GetAimDirection(warlockPosition);
        if (inputDirection != null && SelectedSpellId != null) {
            if (WarlockGame.IsLocal) {
                CommandProcessor.IssueCastCommand(_player.Id, inputDirection.Value, SelectedSpellId.Value);
            }
            else {
                var moveAction = new CastCommand { PlayerId = _player.Id, Location = inputDirection.Value, SpellId = SelectedSpellId.Value};
                NetworkManager.SendPlayerCommand(moveAction);
                if(NetworkManager.IsServer) {
                    CommandProcessor.AddDelayedGameCommand(moveAction, WarlockGame.Frame + NetworkManager.FrameDelay);
                }
            }
        }
    }

    private void OnRightClick(InputManager.InputState inputState) {
        var aimPosition = inputState.GetAimPosition()!.Value;
        if (WarlockGame.IsLocal) {
            CommandProcessor.IssueMoveCommand(_player.Id, aimPosition);
        }
        else {
            var moveAction = new MoveCommand { PlayerId = _player.Id, Location = aimPosition };
            NetworkManager.SendPlayerCommand(moveAction);
            if(NetworkManager.IsServer) {
                CommandProcessor.AddDelayedGameCommand(moveAction, WarlockGame.Frame + NetworkManager.FrameDelay);
            }
        }
        SelectedSpellId = null;
    }
}