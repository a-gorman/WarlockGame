using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.UI.Components.Basic;

namespace WarlockGame.Core.Game.UI.Components;

sealed class Scoreboard : InterfaceComponent {
    private readonly GameRules _gameRule;
    private Grid? _grid;
    private readonly Dictionary<int, TextDisplay> _playerLifeDisplays = new();

    public Scoreboard(GameRules gameRule) {
        _gameRule = gameRule;
    }

    protected override void OnAdd() {
        _gameRule.OnChanged += HandleGameRuleChanged;
    }

    protected override void OnRemove() {
        _gameRule.OnChanged -= HandleGameRuleChanged;
    }

    private void HandleGameRuleChanged(LivesChanged eventArgs) {
        if (eventArgs.Reset) {
            var columnWidth = 90;
            var rowHeight = 25;

            if(_grid != null) RemoveComponent(_grid);
            
            var playerIds = _gameRule.Statuses.Keys.ToList();
            _grid = new Grid(0, 0, 2, columnWidth, playerIds.Count, rowHeight);
            for (int i = 0; i < playerIds.Count; i++) {
                var id = playerIds[i];
                var lives = _gameRule.Statuses[id].Lives;
                var player = PlayerManager.GetPlayer(id);
                if (player == null) continue;
                _grid.AddComponentToCell(new TextDisplay(player.Name) {
                    TextColor = player.Color,
                    TextScale = 0.55f
                }, i, 0);
                var lifeDisplay = new TextDisplay(lives.ToString()) {
                    TextColor = player.Color,
                    TextScale = 0.55f
                };
                _grid.AddComponentToCell(lifeDisplay, i, 1);
                _playerLifeDisplays[id] = lifeDisplay;
            }

            var totalWidth = columnWidth * 2;
            Layout = Layout.WithBoundingBox(10, 15, totalWidth, rowHeight * playerIds.Count, Layout.Alignment.TopRight);
            AddComponent(_grid);

            foreach (var playerId in _playerLifeDisplays.Keys) {
                RecalculatePlayerLifeDisplay(playerId);
            }
        }
        else {
            RecalculatePlayerLifeDisplay(eventArgs.PlayerId);
        }
    }

    private void RecalculatePlayerLifeDisplay(int playerId) {
        var lives = _gameRule.Statuses[playerId].Lives;
        if (lives != 0) {
            _playerLifeDisplays[playerId].Text = lives.ToString();
        } else {
            _playerLifeDisplays[playerId].Text = "Defeated";
        }
    }
}