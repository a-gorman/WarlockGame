using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim.Spell;
using WarlockGame.Core.Game.UI.Components.Basic;

namespace WarlockGame.Core.Game.UI.Components;

class SpellPicker : InterfaceComponent {

    private Basic.Grid _grid;
    private const int Spacing = 5;
    private readonly Vector2 _iconSize = new Vector2(90, 90);
    private readonly int _columns = 5;
    private readonly SpellDefinition[] _spells;
    private readonly HashSet<int> _selections = [];
    private readonly int _maxSelections;
    
    public SpellPicker(IEnumerable<SpellDefinition> spells, int selections) {
        _spells = spells.ToArray();
        BoundingBox = new Rectangle(0, 0, 200, 600);
        _grid = CreateGrid();
        _maxSelections = selections;
    }

    private Basic.Grid CreateGrid() {
        var rows = _spells.Length / _columns + 1;

        var grid = new Basic.Grid(BoundingBox, rows, _columns);
        for (var i = 0; i < _spells.Length; i++) {
            var spell = _spells[i];
            var iColumn = i % _columns;
            var iRow = i / _columns;

            var spellIndex = i;
            var selectionBox = new ToggleSelection(new Rectangle(0, 0, 90, 90), spell.SpellIcon, Color.LimeGreen) {
                LeftClick = (_) => LeftClick(spellIndex)
            };
            
            grid.AddComponent(selectionBox, iRow, iColumn);
        }

        return _grid;
    }

    private void LeftClick(int spellIndex) {
        if (!_selections.Remove(spellIndex)) {
            _selections.Add(spellIndex);
        }

        if (_selections.Count == _maxSelections) {
            // TODO
        }
    }
}