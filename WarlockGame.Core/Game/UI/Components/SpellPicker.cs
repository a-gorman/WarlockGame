using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;
using ZLinq;

namespace WarlockGame.Core.Game.UI.Components;

class SpellPicker : InterfaceComponent {
    private readonly Button _confirmButton;
    private Basic.Grid? _grid;
    private const int Spacing = 5;
    private readonly Vector2 _iconSize = new Vector2(90, 90);
    private readonly int _columns = 5;
    private SpellDefinition[]? _spells;
    private readonly HashSet<int> _selections = [];
    private readonly int _maxSelections;
    
    public SpellPicker(int selections) {
        Clickable = ClickableState.PassThrough;
        BoundingBox = new Rectangle(600, 400, 600, 300);
        _maxSelections = selections;
        var width = 80;
        var height = 40;

        var activeTexture = new Texture2D(Art.Pixel.GraphicsDevice, 1, 1);
        activeTexture.SetData([Color.Green]);
        var inactiveTexture = new Texture2D(Art.Pixel.GraphicsDevice, 1, 1);
        inactiveTexture.SetData([Color.DarkGray]);
        
        _confirmButton = new Button(new Rectangle(600-width-10, 300-height-10, width, height), activeTexture, inactiveTexture) {
            IsActive = false,
            LeftClick = _ => {
                var playerId = PlayerManager.LocalPlayerId;
                if (playerId == null) return;
                
                InputManager.HandlePlayerAction(new SelectSpells { PlayerId = playerId.Value, SpellIds = _selections.Select(x => _spells![x].Id).ToArray() });
                Visible = false;
            }
        };
        AddComponent(_confirmButton);
        
        _confirmButton.AddComponent(new TextDisplay { Text = "Confirm", TextScale = 0.5f });
    }

    public override void Update(ref readonly UIManager.UpdateArgs args) {
        var playerId = PlayerManager.LocalPlayerId;
        if (playerId == null) {
            Visible = false;
            return;
        }

        var sim = WarlockGame.Instance.Simulation;

        if (WasMadeVisible || _spells == null) {
            _spells = sim.GameRules.AvailableSpells
                .Where(x => !sim.GameRules.StartingSpells.Contains(x))
                .Select(x => sim.SpellManager.Definitions[x])
                .ToArray();

            if (_grid != null) {
                RemoveComponent(_grid);
            }
            _grid = CreateGrid();
            AddComponent(_grid);
            Visible = true;
            return;
        }
        
        var force = sim.Forces.AsValueEnumerable().FirstOrDefault(x => x.Id == playerId);

        Visible = (!force?.AreSpellsChosen ?? false) && _spells != null;
    }

    private Basic.Grid CreateGrid() {
        var rows = _spells!.Length / _columns + 1;

        var grid = new Basic.Grid(BoundingBox.AtLocation(20, 20), _columns, rows) { Clickable = ClickableState.PassThrough };
        for (var i = 0; i < _spells.Length; i++) {
            var spell = _spells[i];
            var iColumn = i % _columns;
            var iRow = i / _columns;

            var spellIndex = i;
            var selectionBox = new ToggleSelection(new Rectangle(0, 0, 80, 80), spell.SpellIcon, Color.LimeGreen) {
                LeftClick = _ => LeftClick(spellIndex)
            };
            
            grid.AddComponent(selectionBox, iRow, iColumn);
        }

        return grid;
    }

    private void LeftClick(int spellIndex) {
        if (!_selections.Remove(spellIndex)) {
            _selections.Add(spellIndex);
        }

        _confirmButton.IsActive = _selections.Count == _maxSelections;
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.Draw(Art.Pixel, BoundingBox.WithOffset(location), Color.Maroon);
    }
}