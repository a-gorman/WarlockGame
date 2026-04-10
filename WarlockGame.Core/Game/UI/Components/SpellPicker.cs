using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

class SpellPicker : InterfaceComponent {
    private readonly Button _confirmButton;
    private Grid? _grid;
    private readonly int _columns = 5;
    private SpellDefinition[]? _spells;
    private readonly HashSet<int> _selections = [];
    private readonly int _maxSelections;
    private readonly Simulation _sim;

    public SpellPicker(int selections, Simulation sim) {
        Clickable = ClickableState.PassThrough;
        Layout = Layout.WithSize(600, 300, Alignment.Center);
        _maxSelections = selections;
        _sim = sim;
        var descriptionText = $"Select {selections} spells";

        var activeTexture = new Texture2D(Art.Pixel.GraphicsDevice, 1, 1);
        activeTexture.SetData([Color.Green]);
        var inactiveTexture = new Texture2D(Art.Pixel.GraphicsDevice, 1, 1);
        inactiveTexture.SetData([Color.DarkGray]);
        
        AddComponent(new TextDisplay(descriptionText) { TextScale = 0.75f });
        _confirmButton = new Button(activeTexture, inactiveTexture) {
            IsActive = false,
            Layout = Layout.WithBoundingBox(-10, -10, 80, 40, Alignment.BottomRight),
            LeftClick = _ => {
                var playerId = PlayerManager.LocalPlayerId;
                if (playerId == null) return;
                
                InputManager.HandlePlayerAction(new SelectSpells { PlayerId = playerId.Value, SpellDefIds = _selections.Select(x => _spells![x].Id).ToArray() });
                Visible = false;
            }
        };
        AddComponent(_confirmButton);
        
        _confirmButton.AddComponent(new TextDisplay("Confirm") { TextScale = 0.5f });
        Visible = false;
    }

    public override void Update(ref readonly UIManager.UpdateArgs args) {
        var playerId = PlayerManager.LocalPlayerId;
        if (playerId == null) {
            Visible = false;
        }
    }

    private Grid CreateGrid() {
        var rows = _spells!.Length / _columns + 1;

        var grid = new Grid(BoundingBox.AtOrigin().WithMargin(20), _columns, rows) {
            Clickable = ClickableState.PassThrough
        };
        for (var i = 0; i < _spells.Length; i++) {
            var spell = _spells[i];
            var iColumn = i % _columns;
            var iRow = i / _columns;

            var spellIndex = i;
            var selectionBox = new ToggleSelection(spell.SpellIcon, Color.LimeGreen) {
                Layout = Layout.WithSize(80, 80),
                LeftClick = _ => OnLeftClickSelection(spellIndex)
            };
            
            grid.AddComponentToCell(selectionBox, iRow, iColumn);
        }

        return grid;
    }

    private void OnLeftClickSelection(int spellIndex) {
        if (!_selections.Remove(spellIndex)) {
            _selections.Add(spellIndex);
        }

        _confirmButton.IsActive = _selections.Count == _maxSelections;
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.Draw(Art.Pixel, BoundingBox.WithOffset(location), Color.Maroon);
    }

    protected override void OnAdd() {
        _sim.SimRestarted += OnGameRestart;
    }

    protected override void OnRemove() {
        _sim.SimRestarted -= OnGameRestart;
    }

    private void OnGameRestart() {
        _spells = _sim.GameRules.AvailableSpells
            .Where(x => !_sim.GameRules.StartingSpells.Contains(x))
            .Select(x => _sim.SpellManager.Definitions[x])
            .ToArray();

        if (_grid != null) {
            RemoveComponent(_grid);
        }

        _confirmButton.IsActive = false;
        _selections.Clear();
        _grid = CreateGrid();
        AddComponent(_grid);

        Visible = true;
    }
}