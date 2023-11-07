using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using NeonShooter.Core.Game.UX.InputDevices;
using PS4Mono;

// using PS4Mono;

namespace NeonShooter.Core.Game; 

static class InputDeviceManager {
    private static List<IInputDevice> _devices = new();

    public static void Update() {
        foreach (var device in _devices) {
            device.Update();
        }
        
        Ps4Input.Update();

        if (Ps4Input.Ps4Check(Ps4Input.ConnectedPs4().FirstOrDefault(),Buttons.A)) {
            var x = 5;
        }
    }

    public static void Add(IInputDevice device) {
        _devices.Add(device);
    }
}