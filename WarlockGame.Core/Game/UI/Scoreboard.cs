using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Sim.Rule;
using WarlockGame.Core.Game.UI.Basic;

namespace WarlockGame.Core.Game.UI;

class Scoreboard : InterfaceComponent {
    private readonly MaxLives _gameRule;
    private readonly Basic.Grid _grid;
    private readonly Dictionary<int, TextDisplay> _playerLifeDisplays = new();

    public Scoreboard(MaxLives gameRule) {
        _gameRule = gameRule;
        var playerIds = gameRule.PlayerLives.Keys.ToList();
        _grid = new Basic.Grid(200, 300, 2, 300, playerIds.Count, 100);
        AddComponent(_grid);
        for (int i = 0; i < playerIds.Count; i++) {
            var id = playerIds[i];
            var lives = gameRule.PlayerLives[id];
            var player = PlayerManager.GetPlayer(id);
            if (player == null) continue;
            _grid.AddComponent(new TextDisplay { 
                Bounds = new Rectangle(0,0,200,300), 
                TextColor = player.Color,
                Text = player.Name 
            }, i, 0);
            var lifeDisplay = new TextDisplay { 
                Bounds = new Rectangle(0,0,200,300), 
                TextColor = player.Color,
                Text = lives.ToString()
            };
            _grid.AddComponent(lifeDisplay, i, 1);
            _playerLifeDisplays[id] = lifeDisplay; 
        }
    }

    public override void OnAdd() {
        _gameRule.OnChanged += HandleGameRuleChanged;
    }
    
    public override void OnRemove() {
        _gameRule.OnChanged -= HandleGameRuleChanged;
    }

    private void HandleGameRuleChanged(MaxLivesChanged eventArgs) {
        if(eventArgs.Reset) {
            foreach(var playerId in _playerLifeDisplays.Keys) {
                RecalculatePlayerLives(playerId);
            }
        } else {
            RecalculatePlayerLives(eventArgs.PlayerId);
        }
    }

    private void RecalculatePlayerLives(int playerId) {
        _playerLifeDisplays[playerId].Text = _gameRule.PlayerLives[playerId].ToString();
    }
}