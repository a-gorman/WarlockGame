using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using NeonShooter.Core.Game.UX.InputDevices;
using PS4Mono;

namespace NeonShooter.Core.Game.UX; 

static class InputManager {
    private static List<IInputDevice> _devices = new();
    private static List<LocalPlayerGameInput> _localPlayerInputs = new();

    public static void Update() {
        Ps4Input.Update();

        foreach (var device in _devices) {
            device.Update();
        }

        foreach (var localPlayerInput in _localPlayerInputs) {
            localPlayerInput.Update();
        }
    }

    public static void AddInputDevice(IInputDevice device) {
        _devices.Add(device);
    }
    
    public static void AttachLocalInput(Player player, DeviceType deviceType) {
        var inputDevices = GetDevices(deviceType);
        _devices.AddRange(inputDevices);
        
        _localPlayerInputs.Add(new LocalPlayerGameInput(inputDevices, player));
    }
    
    private static List<IInputDevice> GetDevices(DeviceType deviceType) {
        return deviceType switch
        {
            DeviceType.MouseAndKeyboard => new List<IInputDevice> { new KeyboardInput(), new MouseInput() },
            DeviceType.Gamepad1 => new List<IInputDevice> { new GamepadInput(0) },
            DeviceType.PlayStation1 => new List<IInputDevice> { new PlayStationInput(0) },
            _ => throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, null)
        };
    }
    
    public enum DeviceType {
        MouseAndKeyboard,
        Gamepad1,
        PlayStation1
    }
}