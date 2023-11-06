using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.UX.InputDevices; 

interface IInputDevice {
    public IReadOnlySet<InputAction> GetInputActions();

    public Vector2? Position { get; }

    public Vector2? LeftStick { get; }
    public Vector2? RightStick { get; }
    
    public void Update();
}