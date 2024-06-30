using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PS4Mono;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Input.Devices; 

public class PlayStationInput : IInputDevice {
    private readonly int _controllerIndex;
    private readonly Dictionary<Buttons,GamepadMapping> _mappings;
    private HashSet<InputAction> _actions = new();


    public IReadOnlySet<InputAction> GetInputActions() {
        return _actions;
    }

    public PlayStationInput(int controllerIndex) {
        _controllerIndex = controllerIndex;
        
        _mappings = new List<GamepadMapping>
        {
            new() { DisplayValue = "A", Button = Buttons.A, Action = InputAction.Spell1 },
            new() { DisplayValue = "B", Button = Buttons.B, Action = InputAction.Spell2 },
            new() { DisplayValue = "R1", Button = Buttons.RightShoulder, Action = InputAction.Cancel },
            new() { DisplayValue = "R2", Button = Buttons.RightTrigger, Action = InputAction.Select },
            // new() { DisplayValue = "X", Button = Buttons.X, Action = null }
        }.ToDictionary(x => x.Button);
    }


    public Vector2? Position => null;
    public Vector2? LeftStick { get; private set; }
    public Vector2? RightStick { get; private set; }
    public void Update() {
        _actions.Clear();
        _mappings.Values.Where(x => Ps4Input.Ps4Check(_controllerIndex, x.Button))
                 .ForEach(x => _actions.Add(x.Action));

        LeftStick = Ps4Input.Ps4RawAxis(_controllerIndex, Buttons.LeftStick).Let<Vector2, Vector2?>(x => 
            x.LengthSquared() >= Ps4Input.Ps4AxisDeadZone.Squared() ? x.WithMaxLength(1) : null
        );
        RightStick = Ps4Input.Ps4RawAxis(_controllerIndex, Buttons.RightStick).Let<Vector2, Vector2?>(x => 
            x.LengthSquared() >= Ps4Input.Ps4AxisDeadZone.Squared() ? x.WithMaxLength(1) : null
        );
    }

    private struct GamepadMapping {
        public string DisplayValue { get; set; }
        public InputAction Action { get; set; }
        public Buttons Button { get; set; }
    }
}