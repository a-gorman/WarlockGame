using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Input.Devices; 

public class MouseInput : IInputDevice {
    private readonly Dictionary<InputAction, MouseMapping> _mappings;
    private MouseState _mouseState = Mouse.GetState();
    private readonly HashSet<InputAction> _actions = new();

    public Vector2? Position => _mouseState.Position.ToVector2();
    public Vector2? LeftStick => null;
    public Vector2? RightStick => null;

    public MouseInput() {
        _mappings = new List<MouseMapping>
        {
            new() { DisplayValue = "Left Mouse Button", ButtonSelector = x => x.LeftButton, Action = InputAction.LeftClick },
            new() { DisplayValue = "Right Mouse Button", ButtonSelector = x => x.RightButton, Action = InputAction.RightClick }
        }.ToDictionary(x => x.Action);
    }
    
    public IReadOnlySet<InputAction> GetInputActions() {
        return _actions;
    }

    public void Update() {
        _mouseState = Mouse.GetState();

        _actions.Clear();
        _mappings.Where(x => x.Value.ButtonSelector(_mouseState) == ButtonState.Pressed)
                 .Select(x => x.Key)
                 .ForEach(x => _actions.Add(x));
    }

    private struct MouseMapping {
        public string DisplayValue { get; set; }
        public InputAction Action { get; set; }
        public Func<MouseState,ButtonState> ButtonSelector { get; set; }
    }
}