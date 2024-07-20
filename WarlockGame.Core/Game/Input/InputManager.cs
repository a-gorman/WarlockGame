using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;
using Warlock = WarlockGame.Core.Game.Entity.Warlock;

namespace WarlockGame.Core.Game.Input; 

static class InputManager {
    // private static readonly List<IInputDevice> _devices = new();
    private static readonly MouseInput _mouse = new();
    private static readonly KeyboardInput _keyboard = new();
    private static readonly List<ITextInputConsumer> _textInputConsumers = new();
    private static readonly InputState _inputState = new();
    
    private static LocalPlayerGameInput? _localPlayerInput;

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
            _localPlayerInput?.Update(_inputState);
        }

        _textInputConsumers.RemoveAll(x => x.IsExpired);
    }

    public static void AttachLocalGameInput(Player player) {
        _localPlayerInput = new LocalPlayerGameInput(player);
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
        if (inputState.WasActionKeyPressed(InputAction.Exit))
            WarlockGame.Instance.Exit();

        if (inputState.WasActionKeyPressed(InputAction.Connect) && !NetworkManager.IsConnected) {
            UIManager.OpenTextPrompt("Enter name:", (name, accepted) => {
                if (accepted) {
                    UIManager.OpenTextPrompt("Enter Host IP Address:", (ipAddress, accepted) => {
                        if (accepted && !ipAddress.IsEmpty()) {
                            NetworkManager.ConnectToServer(ipAddress.NullOrEmptyTo("localhost"),
                                () => NetworkManager.JoinGame(name));
                        }
                    });
                }
            });
        }

        if (inputState.WasActionKeyPressed(InputAction.Host) && !NetworkManager.IsConnected) {
            var player = PlayerManager.AddLocalPlayer("Alex");
            EntityManager.Add(new Warlock(player.Id));
            NetworkManager.StartServer();
        }

        if (inputState.WasActionKeyPressed(InputAction.OpenCommandInput)) {
            UIManager.OpenTextPrompt("", (input, accepted) => {
                if (accepted) HandleTextCommand(input);
            });
        }
    }
    
    private static void HandleTextCommand(string input) {
        switch (input.ToLowerInvariant())
        {
            case "restart" or "rm":
                if (!NetworkManager.IsServer) {
                    Logger.Info("Must be server host to restart game"); 
                    return;
                }
                WarlockGame.Instance.RestartGame();
                break;
            case "exit" or "quit" or "q":
                WarlockGame.Instance.Exit();
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