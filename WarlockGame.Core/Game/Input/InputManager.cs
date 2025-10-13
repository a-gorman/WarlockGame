using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;
using KeyboardInput = WarlockGame.Core.Game.Input.Devices.KeyboardInput;

namespace WarlockGame.Core.Game.Input; 

static class InputManager {
    public static int? SelectedSpellId { get; set; }
    public static bool HasTextConsumers => _textInputConsumers.Any();

    private static int? LocalPlayerId => PlayerManager.LocalPlayerId;
    
    private static readonly MouseInput _mouse = new();
    private static KeyboardInput _keyboard = null!;
    private static readonly List<ITextInputConsumer> _textInputConsumers = new();
    private static readonly InputState _inputState = new();
    private static readonly TextCommandHandler _commandHandler = new();

    private static readonly List<InputAction> SpellSelectionActions = new() {
        InputAction.Spell1, InputAction.Spell2, InputAction.Spell3, InputAction.Spell4, InputAction.Spell5, 
        InputAction.Spell6, InputAction.Spell7, InputAction.Spell8, InputAction.Spell9, InputAction.Spell10
    };

    public static void Initialize(Dictionary<Keys, InputAction> keyMappings) {
        _commandHandler.Initialize();
        _keyboard = new KeyboardInput(keyMappings);
    }
    
    public static InputState Update() {
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
        }

        _textInputConsumers.RemoveAll(x => x.IsExpired);

        return _inputState;
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
            UIManager.OpenTextPrompt("", x => _commandHandler.HandleCommand(x));
        }

        if (LocalPlayerId is null) return;
        
        if (!HasTextConsumers) {
            var sim = WarlockGame.Instance.Simulation;
            foreach (var actionType in SpellSelectionActions) {
                if (inputState.WasActionKeyPressed(actionType)) {
                    var selectedSpell = sim.SpellManager.PlayerSpells[LocalPlayerId.Value]
                        .FirstOrDefault(x => x.Value.SlotLocation == SpellSelectionActions.IndexOf(actionType)).Value;
                    selectedSpell?.Effect.Switch(
                        _ => SelectedSpellId = selectedSpell.Id,
                        _ => SelectedSpellId = selectedSpell.Id,
                        _ => HandlePlayerAction(new CastAction { PlayerId = LocalPlayerId.Value, Type = CastAction.CastType.Self, SpellId = selectedSpell.Id })
                    );
                }
            }
        }
        
        if(inputState.WasActionKeyPressed(InputAction.LeftClick)) UIManager.HandleLeftClick(inputState.GetMousePosition());
        if(inputState.WasActionKeyPressed(InputAction.RightClick)) UIManager.HandleRightClick(inputState.GetMousePosition());
    }

    public static void HandlePlayerAction<T>(T command)  where T : IPlayerAction, new() {
        if (NetworkManager.IsClient) {
            NetworkManager.SendSerializable(command);
        }
        else {
            CommandManager.AddSimulationCommand(command);
        }
    }
    
    public class InputState {
        private HashSet<InputAction> _actions = new();
        private HashSet<InputAction> _previousActions = new();
        private Vector2 _mousePosition;
        private Vector2 _previousMousePosition;

        internal InputState() {}
        
        public void Clear() {
            _actions.Clear();
            _previousActions.Clear();
        }

        public void Update(IEnumerable<InputAction> actions, Vector2 mouseLocation) {
            (_actions, _previousActions) = (_previousActions, _actions);
    
            _actions.Clear();
            _actions.UnionWith(actions);
            _previousMousePosition = _mousePosition;
            _mousePosition = mouseLocation;
        }
        
        public bool IsActionKeyDown(InputAction action) => _actions.Contains(action);

        public bool WasActionKeyPressed(InputAction action) => _actions.Contains(action) && !_previousActions.Contains(action);
        
        public Vector2 GetAimDirection(Vector2 relativeTo) {
            return (_mousePosition - relativeTo).ToNormalizedOrZero();
        }

        public Vector2 GetMousePosition() {
            return _mousePosition;
        }
    }
}