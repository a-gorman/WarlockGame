using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.UI.Components.Basic;

namespace WarlockGame.Core.Game.UI.Components;

sealed class Scoreboard : InterfaceComponent {
    private readonly GameRules _gameRule;
    private Components.Basic.Grid _grid;
    private readonly Dictionary<int, TextDisplay> _playerLifeDisplays = new();

    public Scoreboard(GameRules gameRule) {
        _grid = new Basic.Grid(0, 0, 1, 1, 1, 1);
        AddComponent(_grid);
        _gameRule = gameRule;
    }

    public override void OnAdd() {
        _gameRule.OnChanged += HandleGameRuleChanged;
    }
    
    public override void OnRemove() {
        _gameRule.OnChanged -= HandleGameRuleChanged;
    }

    private void HandleGameRuleChanged(LivesChanged eventArgs) {
        if (eventArgs.Reset) {
            var columnWidth = 90;
            var rowHeight = 25;

            RemoveComponent(_grid);
            var playerIds = _gameRule.Statuses.Keys.ToList();
            _grid = new Components.Basic.Grid(0, 0, 2, columnWidth, playerIds.Count, rowHeight);
            for (int i = 0; i < playerIds.Count; i++) {
                var id = playerIds[i];
                var lives = _gameRule.Statuses[id].Lives;
                var player = PlayerManager.GetPlayer(id);
                if (player == null) continue;
                _grid.AddComponent(new TextDisplay {
                    TextColor = player.Color,
                    Text = player.Name,
                    TextScale = 0.55f
                }, i, 0);
                var lifeDisplay = new TextDisplay {
                    TextColor = player.Color,
                    Text = lives.ToString(),
                    TextScale = 0.55f
                };
                _grid.AddComponent(lifeDisplay, i, 1);
                _playerLifeDisplays[id] = lifeDisplay;
            }

            var totalWidth = columnWidth * 2;
            BoundingBox = new Rectangle((int)WarlockGame.ScreenSize.X - totalWidth, 15, totalWidth,
                rowHeight * playerIds.Count);
            AddComponent(_grid);

            foreach (var playerId in _playerLifeDisplays.Keys) {
                RecalculatePlayerLives(playerId);
            }
        }
        else {
            RecalculatePlayerLives(eventArgs.PlayerId);
        }
    }

    private void RecalculatePlayerLives(int playerId) {
        _playerLifeDisplays[playerId].Text = _gameRule.Statuses[playerId].Lives.ToString();
    }
}