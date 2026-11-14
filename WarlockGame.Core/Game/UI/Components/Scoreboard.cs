using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.UI.Components.Basic;

namespace WarlockGame.Core.Game.UI.Components;

sealed class Scoreboard : InterfaceComponent {
    private readonly GameRules _gameRule;
    private Container _container;
    private readonly Dictionary<int, TextDisplay> _forceLifeDisplays = new();

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
            
            _forceLifeDisplays.Clear();
            var forceIds = _gameRule.Statuses.Keys.ToList();
            var grid = new Grid(0, 0, 2, columnWidth, forceIds.Count, rowHeight) { Layout = Layout.WithMargin(2) };
            for (int i = 0; i < forceIds.Count; i++) {
                var id = forceIds[i];
                var lives = _gameRule.Statuses[id].Lives;
                var player = PlayerManager.GetPlayerByForceId(id);
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
                _forceLifeDisplays[id] = lifeDisplay;
            }

            var totalWidth = columnWidth * 2;
            Layout = Layout.WithBoundingBox(10, 15, totalWidth, rowHeight * forceIds.Count, Alignment.TopRight);
            _container.AddComponent(grid);

            foreach (var playerId in _forceLifeDisplays.Keys) {
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
            _forceLifeDisplays[playerId].Text = lives.ToString();
        } else {
            _forceLifeDisplays[playerId].Text = "Defeated";
        }
    }
}