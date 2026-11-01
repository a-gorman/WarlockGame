using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.UI.Components.Basic;

namespace WarlockGame.Core.Game.UI.Components;

sealed class Scoreboard : InterfaceComponent {
    private readonly GameRules _gameRule;
    private Container _container;
    private readonly Dictionary<int, TextDisplay> _playerLifeDisplays = new();

    public Scoreboard(GameRules gameRule) {
        _gameRule = gameRule;
        Layout = Layout.WithMargin(5);
        Visible = false;
        _container = new Container { BackgroundColor = Color.Tan, BorderColor = Color.Black, BorderWidth = 2 };
        AddComponent(_container);
    }

    protected override void OnAdd() {
        _gameRule.OnChanged += HandleGameRuleChanged;
    }

    protected override void OnRemove() {
        _gameRule.OnChanged -= HandleGameRuleChanged;
    }

    private void HandleGameRuleChanged(LivesChanged eventArgs) {
        Visible = true;
        if (eventArgs.Reset) {
            var columnWidth = 90;
            var rowHeight = 25;

            _container.RemoveAllComponents();
            
            var playerIds = _gameRule.Statuses.Keys.ToList();
            var grid = new Grid(0, 0, 2, columnWidth, playerIds.Count, rowHeight) { Layout = Layout.WithMargin(2) };
            for (int i = 0; i < playerIds.Count; i++) {
                var id = playerIds[i];
                var lives = _gameRule.Statuses[id].Lives;
                var player = PlayerManager.GetPlayer(id);
                if (player == null) continue;
                grid.AddComponentToCell(new TextDisplay(player.Name) {
                    TextColor = player.Color,
                    TextScale = 0.55f
                }, i, 0);
                var lifeDisplay = new TextDisplay(lives.ToString()) {
                    TextColor = player.Color,
                    TextScale = 0.55f
                };
                grid.AddComponentToCell(lifeDisplay, i, 1);
                _playerLifeDisplays[id] = lifeDisplay;
            }

            var totalWidth = columnWidth * 2;
            Layout = Layout.WithBoundingBox(10, 15, totalWidth, rowHeight * playerIds.Count, Alignment.TopRight);
            _container.AddComponent(grid);

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