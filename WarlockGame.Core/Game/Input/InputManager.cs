using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PS4Mono;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.UI;

namespace WarlockGame.Core.Game.Input; 

static class InputManager {
    private static readonly List<IInputDevice> _devices = new();
    private static readonly List<LocalPlayerGameInput> _localPlayerInputs = new();
    private static readonly List<ITextInputConsumer> _textInputConsumers = new();
    
    public static bool HasTextConsumers => _textInputConsumers.Any();

    public static void Update() {
        Ps4Input.Update();

        foreach (var device in _devices) {
            device.Update();
        }

        foreach (var localPlayerInput in _localPlayerInputs) {
            localPlayerInput.Update();
        }
    }

    public static void AddTextConsumer(ITextInputConsumer consumer) {
        _textInputConsumers.Add(consumer);
        // Sort higher priority consumers to the front
        _textInputConsumers.Sort((first,second) => second.TextConsumerPriority.CompareTo(first.TextConsumerPriority));
        consumer.OnClose += HandledOnClosedTextConsumer;
    }
    
    public static void OnTextInput(TextInputEventArgs args) {
        // Favor newer items
        _textInputConsumers.FirstOrDefault()?.OnTextInput(args);
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
            // DeviceType.Gamepad => new List<IInputDevice> { new GamepadInput(0) },
            // DeviceType.PlayStation => new List<IInputDevice> { new PlayStationInput(0) },
            _ => throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, null)
        };
    }

    private static void HandledOnClosedTextConsumer(object? sender, EventArgs eventArgs) {
        if (sender is ITextInputConsumer consumer) {
            _textInputConsumers.Remove(consumer);
            consumer.OnClose -= HandledOnClosedTextConsumer;
        }
    }
    
    public enum DeviceType {
        MouseAndKeyboard,
        // Gamepad,
        // PlayStation
    }
}