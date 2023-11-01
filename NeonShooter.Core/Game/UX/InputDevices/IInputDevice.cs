using System.Collections.Generic;

namespace NeonShooter.Core.Game.UX.InputDevices; 

interface IInputDevice {
    public bool WasActionKeyPressed(InputAction action);

    public bool WasActionReleased(InputAction action);

    public bool IsActionKeyDown(InputAction action);

    public IReadOnlySet<InputAction> GetReleasedActions();
    
    public IReadOnlySet<InputAction> GetPressedActions();
    
    public IReadOnlySet<InputAction> GetHeldActions();
}