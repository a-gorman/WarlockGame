using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game.UX.InputDevices; 

interface IInputDevice {
    public IReadOnlySet<InputAction> GetInputActions();

    public Vector2? Position { get; }
    
    public void Update();
}