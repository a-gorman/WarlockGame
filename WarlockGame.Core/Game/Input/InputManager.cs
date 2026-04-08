using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.UI;
using KeyboardInput = WarlockGame.Core.Game.Input.Devices.KeyboardInput;

namespace WarlockGame.Core.Game.Input; 

static class InputManager {
    public static int? SelectedSpellId { get; set; }
    public static bool HasTextConsumers => _textInputConsumers.Any();

    public static readonly InputState LastInputState = new();
    
    private static int? LocalPlayerId => PlayerManager.LocalPlayerId;
    
    private static readonly MouseInput _mouse = new();
    private static KeyboardInput _keyboard = null!;
    private static readonly List<ITextInputConsumer> _textInputConsumers = new();
    private static readonly ConsoleCommandHandler _commandHandler = new();

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
            LastInputState.Update(_mouse.GetInputActions().Union(_keyboard.GetInputActions()), _mouse.Position);
        }
        else {
            LastInputState.Clear();
        }
        
        if (!HasTextConsumers) {
            HandleGameFunctions(LastInputState);
        }

        if(LastInputState.WasActionKeyPressed(InputAction.LeftClick)) UIManager.HandleLeftClick(LastInputState.GetMousePosition());
        if(LastInputState.WasActionKeyPressed(InputAction.RightClick)) UIManager.HandleRightClick(LastInputState.GetMousePosition());
        
        _textInputConsumers.RemoveAll(x => x.IsExpired);

        return LastInputState;
    }

    public static void AddTextConsumer(ITextInputConsumer consumer) {
        _textInputConsumers.Add(consumer);
        // Sort higher priority consumers to the front
        _textInputConsumers.Sort((first,second) => second.TextConsumerPriority.CompareTo(first.TextConsumerPriority));
    }
    
    public static void RemoveTextConsumer(ITextInputConsumer consumer) {
        _textInputConsumers.Remove(consumer);
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

        if (!HasTextConsumers && LocalPlayerId is not null) {
            var sim = WarlockGame.Instance.Simulation;
            foreach (var actionType in SpellSelectionActions) {
                if (inputState.WasActionKeyPressed(actionType) && sim.SpellManager.PlayerSpells.TryGetValue(LocalPlayerId.Value, out var localPlayerSpells)) {
                    var selectedSpell = localPlayerSpells?.FirstOrDefault(x => x.Value.SlotLocation == SpellSelectionActions.IndexOf(actionType)).Value;
                    selectedSpell?.Effect.Switch(
                        _ => SelectedSpellId = selectedSpell.Id,
                        _ => SelectedSpellId = selectedSpell.Id,
                        _ => HandlePlayerAction(new CastAction { PlayerId = LocalPlayerId.Value, Type = CastAction.CastType.Self, SpellId = selectedSpell.Id })
                    );
                    Logger.Debug($"Selected spell: Id: {selectedSpell?.Id} Name: {selectedSpell?.Definition.Name}", Logger.LogType.PlayerAction | Logger.LogType.Interface);
                }
            }
        }
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

        public Vector2 GetMousePosition() {
            return _mousePosition;
        }
    }
}