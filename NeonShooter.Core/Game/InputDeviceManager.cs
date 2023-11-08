using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using NeonShooter.Core.Game.UX.InputDevices;
using PS4Mono;

// using PS4Mono;

namespace NeonShooter.Core.Game; 

static class InputDeviceManager {
    private static List<IInputDevice> _devices = new();

    public static void Update() {
        Ps4Input.Update();

        foreach (var device in _devices) {
            device.Update();
        }
    }

    public static void Add(IInputDevice device) {
        _devices.Add(device);
    }
}