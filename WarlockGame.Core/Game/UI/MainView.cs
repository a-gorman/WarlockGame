using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

sealed class MainView : InterfaceComponent {
    private readonly Simulation _sim;
    
    public MainView(Simulation sim) {
        _sim = sim;
        Layer = -1;
        Clickable = ClickableState.Consume;
        BoundingBox = new Rectangle(new Point(0, 0), WarlockGame.ScreenSize.ToPoint());
    }

    public override void OnRightClick(Vector2 location) {
        var localPlayerId = PlayerManager.LocalPlayer?.Id;
        if (localPlayerId != null) {
            InputManager.IssueCommand(new MoveCommand { PlayerId = localPlayerId.Value, Location = location });
        }
        InputManager.SelectedSpellId = null;
        return;
    }

    public override void OnLeftClick(Vector2 location) {
        if (InputManager.SelectedSpellId != null) {
            var localPlayerId = PlayerManager.LocalPlayer?.Id;
            if (localPlayerId == null) return;
            
            var warlock = _sim.EntityManager.GetWarlockByPlayerId(localPlayerId.Value);
            if (warlock == null) return;
            if (!_sim.SpellManager.Spells.TryGetValue(InputManager.SelectedSpellId.Value, out var spell)) return;

            Vector2? castVector = spell.Effect.Match<Vector2?>(
                _ => (location - warlock.Position).ToNormalizedOrZero(),
                _ => location,
                _ => null
            );
            if (castVector is not null) {
                InputManager.IssueCommand(new CastCommand
                    { PlayerId = localPlayerId.Value, 
                        CastVector = castVector.Value, 
                        SpellId = InputManager.SelectedSpellId.Value });
            }
        }

        InputManager.SelectedSpellId = null;
    }
}