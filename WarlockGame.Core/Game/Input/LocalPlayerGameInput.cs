using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.UI;

namespace WarlockGame.Core.Game.Input; 

/// <summary>
/// Handles game input from a particular player
/// </summary>
class LocalPlayerGameInput {
    private static readonly List<InputAction> SpellSelectionActions = new() {
        InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4, InputAction.Spell5, 
        InputAction.Spell6, InputAction.Spell7, InputAction.Spell8, InputAction.Spell9, InputAction.Spell10
    };

    public int? SelectedSpellId { get; set; }

    private readonly int _playerId;

    public LocalPlayerGameInput(int playerId) {
        _playerId = playerId;
    }
    
    public void Update(InputManager.InputState inputState) {
        var sim = WarlockGame.Instance.Simulation;
        var warlock = sim.EntityManager.GetWarlockByPlayerId(_playerId);
        if (warlock is null) return;
        
        if (!InputManager.HasTextConsumers) {
            foreach (var actionType in SpellSelectionActions) {
                if (inputState.WasActionKeyPressed(actionType)) {
                    var selectedSpell = sim.SpellManager.PlayerSpells[_playerId]
                        .FirstOrDefault(x => x.Value.SlotLocation == SpellSelectionActions.IndexOf(actionType)).Value;
                    selectedSpell?.Effect.Switch(
                        _ => SelectedSpellId = selectedSpell.Id,
                        _ => SelectedSpellId = selectedSpell.Id,
                        _ => IssueCommand(new CastCommand { PlayerId = _playerId, CastVector = Vector2.Zero, SpellId = selectedSpell.Id })
                    );
                }
            }
        }
        
        if(inputState.WasActionKeyPressed(InputAction.LeftClick)) OnLeftClick(inputState, warlock.Position);
        if(inputState.WasActionKeyPressed(InputAction.RightClick)) OnRightClick(inputState);
    }

    private void OnLeftClick(InputManager.InputState inputState, Vector2 warlockPosition) {
        if (UIManager.HandleClick(inputState.GetAimPosition()!.Value)) return;
        if (SelectedSpellId != null) {
            var sim = WarlockGame.Instance.Simulation;
            var warlock = sim.EntityManager.GetWarlockByPlayerId(_playerId);
            if (warlock == null) return;
            if(!sim.SpellManager.Spells.TryGetValue(SelectedSpellId.Value, out var spell)) return;

            Vector2? castVector = spell.Effect.Match(
                _ => inputState.GetAimDirection(warlockPosition),
                _ => inputState.GetAimPosition(),
                _ => null
            );
            if (castVector is not null) {
                IssueCommand(new CastCommand
                    { PlayerId = _playerId, CastVector = castVector.Value, SpellId = SelectedSpellId.Value });
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