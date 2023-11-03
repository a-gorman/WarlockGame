using System.Collections.Generic;
using NeonShooter.Core.Game.UX.InputDevices;

namespace NeonShooter.Core.Game; 

static class InputDeviceManager {
    private static List<IInputDevice> _devices = new();

    public static void Update() {
        foreach (var device in _devices) {
            device.Update();
        }
    }

    public static void Add(IInputDevice device) {
        _devices.Add(device);
    }
}