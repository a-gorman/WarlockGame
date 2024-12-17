using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;
using Warlock = WarlockGame.Core.Game.Sim.Entity.Warlock;

namespace WarlockGame.Core.Game.Input; 

static class InputManager {
    // private static readonly List<IInputDevice> _devices = new();
    private static readonly MouseInput _mouse = new();
    private static readonly KeyboardInput _keyboard = new();
    private static readonly List<ITextInputConsumer> _textInputConsumers = new();
    private static readonly InputState _inputState = new();
    
    public static LocalPlayerGameInput? LocalPlayerInput { get; private set; }

    public static bool HasTextConsumers => _textInputConsumers.Any();

    public static void Initialize() {
    }
    
    public static void Update() {
        _mouse.Update();
        _keyboard.Update();

        if (WarlockGame.Instance.IsActive) {
            _inputState.Update(_mouse.GetInputActions().Union(_keyboard.GetInputActions()), _mouse.Position);
        }
        else {
            _inputState.Clear();
        }
        
        if (!HasTextConsumers) {
            HandleGameFunctions(_inputState);
            LocalPlayerInput?.Update(_inputState);
        }

        _textInputConsumers.RemoveAll(x => x.IsExpired);
    }

    public static void AttachLocalGameInput(Player player) {
        LocalPlayerInput = new LocalPlayerGameInput(player.Id);
    }
    
    public static void AddTextConsumer(ITextInputConsumer consumer) {
        _textInputConsumers.Add(consumer);
        // Sort higher priority consumers to the front
        _textInputConsumers.Sort((first,second) => second.TextConsumerPriority.CompareTo(first.TextConsumerPriority));
    }

    public static void OnTextInput(TextInputEventArgs args) {
        // Favor newer items
        _textInputConsumers.FirstOrDefault()?.OnTextInput(args);
    }
    
    /// <summary>
    /// Handle game functions like exiting, opening the command box and joining a server
    /// </summary>
    private static void HandleGameFunctions(InputState inputState) {
        if (inputState.WasActionKeyPressed(InputAction.Exit)) {
            WarlockGame.Instance.Exit();
        }
        
        if (inputState.WasActionKeyPressed(InputAction.OpenCommandInput)) {
            UIManager.OpenTextPrompt("", HandleTextCommand);
        }
    }
    
    private static void HandleTextCommand(string input) {
        switch (input.ToLowerInvariant())
        {
            case "restart" or "rm":
                if (NetworkManager.IsClient) {
                    Logger.Info("Must be server host to restart game"); 
                    return;
                }
                if (WarlockGame.IsLocal) {
                    WarlockGame.Instance.RestartGame(Random.Shared.Next());
                }
                else if(NetworkManager.IsServer) {
                    CommandProcessor.AddDelayedServerCommand(
                        new StartGame { Seed = Random.Shared.Next() }, 
                        Simulation.Instance.Tick + NetworkManager.FrameDelay);
                }
                break;
            
            case "exit" or "quit" or "q":
                WarlockGame.Instance.Exit();
                break;
            
            case "host":
                if (NetworkManager.IsConnected) {
                    Logger.Info("Already in game!");
                    return;
                }
                
                UIManager.OpenTextPrompt("Enter name:", name => {
                    PlayerManager.AddLocalPlayer(name);
                    // EntityManager.Add(new Warlock(player.Id));
                    NetworkManager.StartServer();
                });
                break;
            
            case "join":
                if (NetworkManager.IsConnected) {
                    Logger.Info("Already in game!");
                    return;
                }
                UIManager.OpenTextPrompt("Enter name:", name => {
                    UIManager.OpenTextPrompt("Enter Host IP Address:", ipAddress => {
                        NetworkManager.ConnectToServer(ipAddress.NullOrEmptyTo("localhost"), () => NetworkManager.JoinGame(name));
                    });
                });
                break;
            
            case "check" or "checksum":
                Logger.Info($"Checksum is: {Simulation.Instance.Checksum}");
                break;
            
            case "h" or "help":
                Logger.Info("Commands:");
                Logger.Info("exit");
                Logger.Info("join");
                Logger.Info("host");
                Logger.Info("restart");
                break;
            
            case "logs":
                LogDisplay.Instance.Visible = !LogDisplay.Instance.Visible;
                break;
            
            default:
                Logger.Info($"\"{input}\" not recognized as valid command");
                break;
        };
    }

    public class InputState {
        private HashSet<InputAction> _actions = new();
        private HashSet<InputAction> _previousActions = new();
        private Vector2? _mousePosition = null;
        private Vector2? _previousMousePosition = null;

        internal InputState() {}
        
        public void Clear() {
            _actions.Clear();
            _previousActions.Clear();
            _mousePosition = null;
            _previousMousePosition = null;
        }

        public void Update(IEnumerable<InputAction> actions, Vector2? mouseLocation) {
            (_actions, _previousActions) = (_previousActions, _actions);
    
            _actions.Clear();
            _actions.UnionWith(actions);
            _previousMousePosition = _mousePosition;
            _mousePosition = mouseLocation;
        }
        
        public bool IsActionKeyDown(InputAction action) => _actions.Contains(action);

        public bool WasActionKeyPressed(InputAction action) => _actions.Contains(action) && !_previousActions.Contains(action);
        
        public Vector2? GetAimDirection(Vector2 relativeTo) {
            return (_mousePosition - relativeTo)?.ToNormalizedOrZero();
        }

        public Vector2? GetAimPosition() {
            return _mousePosition;
        }
    }
}