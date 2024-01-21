using System;
using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.UX.InputDevices;

namespace NeonShooter.Core.Game; 

static class PlayerManager {
    public static List<Player> Players { get; } = new();

    public static Player ActivePlayer => Players.First();

    public static void AddPlayer(string name, int id, DeviceType deviceType) {
        var inputDevices = GetDevices(deviceType);
        inputDevices.ForEach(InputDeviceManager.Add);

        var player = new Player(name, id, inputDevices);
        var warlock = new Warlock(player.Id, player.Id);
        player.Warlock = warlock;

        Players.Add(player);
        EntityManager.Add(warlock);

        player.Initialize();
    }

    public static void AddPlayer(Player player) {
        Players.Add(player);
        EntityManager.Add(player.Warlock);
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