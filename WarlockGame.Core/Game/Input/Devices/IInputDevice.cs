using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Input.Devices; 

interface IInputDevice {
    public IReadOnlySet<InputAction> GetInputActions();

    public Vector2? Position { get; }
    
    public void Update();
}