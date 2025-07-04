

public class Scoreboard : InterfaceComponent {
    private readonly MaxLives _gameRule;
    private readonly Grid _grid;
    private readonly Dictionary<int,int> PlayerRows = new();

    public Scoreboard(MaxLives gameRule) {
        _gameRule = gameRule;
        var playerIds = gameRule.PlayerLives.keys;
        _grid = new Grid(200, 300, 2, 300, playerIds.Count, 100)
        AddComponent(_grid);
        for (int i = 0; i < playerIds.Count; i++) {
            var id = playerIds[i];
            var lives = gameRules.PlayerLives[id];
            var player = PlayerManager.GetPlayer(id);
            _grid.AddComponent(new TextDisplay() { 
                Bounds = new Rectangle(0,0,200,300), 
                player.color,
                Text = player.Name 
            }, i, 0);
            _grid.AddComponent(new TextDisplay() { 
                Bounds = new Rectangle(0,0,200,300), 
                Color = player.Color,
                Text = lives.toString(); 
            }, i, 1);
            _playerRows[id] = i; 
        }
    }

    public override OnAdd() {
        _gameRule.OnChanged += HandleGameRuleChanged;
    }

    public override OnRemove() {
        _gameRule.OnChanged -= HandleGameRuleChanged;
    }

    private void HandleGameRuleChanged(MaxLivesChanged eventArgs) {
        if(eventArgs.Reset) {
            foreach(var playerId in PlayerRows.keys) {
                RecalculatePlayerLives(playerId);
            }
        } else {
            RecalculatePlayerLives(eventArgs.PlayerId);
        }
    }

    private void RecalculatePlayerLives(int playerId) {
        var row = _playerRows[playerId];
        // Gross
        (_grid[1, row].Components.Single() as TextDisplayer)!.Text = _gameRule.PlayerLives[playerId].ToString();
    }
}