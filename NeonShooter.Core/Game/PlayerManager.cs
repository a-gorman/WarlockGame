using System;
using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.UX.InputDevices;

namespace NeonShooter.Core.Game; 

static class PlayerManager {
    public static List<Player> Players { get; } = new();

    public static Player ActivePlayer => Players.First();

    public static void AddPlayer(string name, DeviceType deviceType) {
        var inputDevices = GetDevices(deviceType);
        inputDevices.ForEach(InputDeviceManager.Add);

        var player = new Player(inputDevices) { Name = name };
        var playerShip = new PlayerShip(player);
        player.Warlock = playerShip;

        Players.Add(player);
        EntityManager.Add(playerShip);

        player.Initialize();
    }

    public static void Update() {
        foreach (var player in Players) {
            player.Update();
        }
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