using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;

namespace WarlockGame.Core.Game.Input; 

/// <summary>
/// Handles game input from a particular player
/// </summary>
class LocalPlayerGameInput {
    private static readonly List<InputAction> SpellSelectionActions = new() { InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4, InputAction.Spell5 };

    public int? SelectedSpellId { get; private set; }

    private readonly int _playerId;

    public LocalPlayerGameInput(int playerId) {
        _playerId = playerId;
    }
    
    public void Update(InputManager.InputState inputState) {
        var warlock = WarlockGame.Instance?.Simulation.EntityManager.GetWarlockByPlayerId(_playerId);
        if (warlock is null) return;
        
        if (!InputManager.HasTextConsumers) {
            foreach (var actionType in SpellSelectionActions) {
                if (inputState.WasActionKeyPressed(actionType)) {
                    var selectedSpell = warlock.Spells.ElementAtOrDefault(SpellSelectionActions.IndexOf(actionType));
                    selectedSpell?.Effect.Switch(
                        _ => SelectedSpellId = selectedSpell.SpellId,
                        _ => SelectedSpellId = selectedSpell.SpellId,
                        _ => IssueCommand(new CastCommand { PlayerId = _playerId, CastVector = Vector2.Zero, SpellId = selectedSpell.SpellId })
                    );
                }
            }
        }
        
        if(inputState.WasActionKeyPressed(InputAction.LeftClick)) OnLeftClick(inputState, warlock.Position);
        if(inputState.WasActionKeyPressed(InputAction.RightClick)) OnRightClick(inputState);
    }

    private void OnLeftClick(InputManager.InputState inputState, Vector2 warlockPosition) {
        if (SelectedSpellId != null) {
            Vector2? castVector = WarlockGame.Instance?.Simulation
                                             .EntityManager.GetWarlockByPlayerId(_playerId)
                                             !.Spells
                                             .Find(x => x.SpellId == SelectedSpellId)
                                             ?.Effect
                                             .Match(
                                                 _ => inputState.GetAimDirection(warlockPosition),
                                                 _ => inputState.GetAimPosition(),
                                                 _ => null
                                             );
            if (castVector is not null) {
                IssueCommand(new CastCommand { PlayerId = _playerId, CastVector = castVector.Value, SpellId = SelectedSpellId.Value });
            }
        }

        SelectedSpellId = null;
    }

    private void OnRightClick(InputManager.InputState inputState) {
        var aimPosition = inputState.GetAimPosition()!.Value;
        IssueCommand(new MoveCommand { PlayerId = _playerId, Location = aimPosition });
        SelectedSpellId = null;
    }

    private void IssueCommand<T>(T command)  where T : IPlayerCommand, new() {
        if (NetworkManager.IsClient) {
            NetworkManager.SendSerializable(command);
        }
        else {
            CommandManager.AddSimulationCommand(command);
        }
    }
}