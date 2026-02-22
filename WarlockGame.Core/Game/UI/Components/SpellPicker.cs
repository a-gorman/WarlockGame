using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;
using ZLinq;

namespace WarlockGame.Core.Game.UI.Components;

class SpellPicker : InterfaceComponent {
    private readonly Button _confirmButton;
    private Basic.Grid? _grid;
    private readonly int _columns = 5;
    private SpellDefinition[]? _spells;
    private readonly HashSet<int> _selections = [];
    private readonly int _maxSelections;
    private readonly Simulation _sim;

    public SpellPicker(int selections, Simulation sim) {
        Clickable = ClickableState.PassThrough;
        BoundingBox = new Rectangle(0, 0, 600, 300);
        _maxSelections = selections;
        _sim = sim;
        var descriptionText = $"Select {selections} spells";

        var activeTexture = new Texture2D(Art.Pixel.GraphicsDevice, 1, 1);
        activeTexture.SetData([Color.Green]);
        var inactiveTexture = new Texture2D(Art.Pixel.GraphicsDevice, 1, 1);
        inactiveTexture.SetData([Color.DarkGray]);
        
        AddComponent(new TextDisplay { Text = descriptionText, TextScale = 0.75f });
        _confirmButton = new Button(new Rectangle(-10, -10, 80, 40), activeTexture, inactiveTexture) {
            IsActive = false,
            LeftClick = _ => {
                var playerId = PlayerManager.LocalPlayerId;
                if (playerId == null) return;
                
                InputManager.HandlePlayerAction(new SelectSpells { PlayerId = playerId.Value, SpellIds = _selections.Select(x => _spells![x].Id).ToArray() });
                Visible = false;
            }
        };
        AddComponent(_confirmButton, Alignment.BottomRight);
        
        _confirmButton.AddComponent(new TextDisplay { Text = "Confirm", TextScale = 0.5f });
        Visible = false;
    }

    public override void Update(ref readonly UIManager.UpdateArgs args) {
        var playerId = PlayerManager.LocalPlayerId;
        if (playerId == null) {
            Visible = false;
        }
    }

    private Basic.Grid CreateGrid() {
        var rows = _spells!.Length / _columns + 1;

        var grid = new Basic.Grid(BoundingBox.AtOrigin().WithMargin(20), _columns, rows) { Clickable = ClickableState.PassThrough };
        for (var i = 0; i < _spells.Length; i++) {
            var spell = _spells[i];
            var iColumn = i % _columns;
            var iRow = i / _columns;

            var spellIndex = i;
            var selectionBox = new ToggleSelection(new Rectangle(0, 0, 80, 80), spell.SpellIcon, Color.LimeGreen) {
                LeftClick = _ => OnLeftClickSelection(spellIndex)
            };
            
            grid.AddComponent(selectionBox, iRow, iColumn);
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

    public override void OnAdd() {
        _sim.SimRestarted += OnGameRestart;
    }

    public override void OnRemove() {
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
        AddComponent(_grid, Alignment.Center);

        Visible = true;
    }
}