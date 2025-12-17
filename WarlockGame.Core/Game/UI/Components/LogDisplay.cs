using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

sealed class LogDisplay : InterfaceComponent {

    public static LogDisplay Instance { get; } = new LogDisplay();

    public Logger.Level DisplayLevel {
        get;
        set {
            field = value;
            IsDirty = true;
        }
    } = Logger.Level.INFO;

    private readonly TextDisplay _textDisplay;

    private LogDisplay() {
        BoundingBox = new Rectangle(0, 0, 900, 100);
        _textDisplay = new TextDisplay();

        AddComponent(_textDisplay);
    }

    public override void OnAdd() {
        Logger.LogCreated += OnLogAdded;
        Refresh();
    }

    public override void OnRemove() {
        Logger.LogCreated -= OnLogAdded;
    }

    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (IsDirty) {
            Refresh();
            IsDirty = false;
        }
    }

    private void OnLogAdded(Logger.Log _) {
        Refresh();
    }
    
    private void Refresh() {
        if (!Visible) return;
        
        var logs = Logger.Logs
                         .Where(x => x.Level >= DisplayLevel)
                         .Select(FormatLog)
                         .Take(5)
                         .JoinToString('\n');
        _textDisplay.Text = logs;
    }

    private string FormatLog(Logger.Log log)
    {
        if (log.DedupCount == 0) {
            return $"{log.LevelString()} [{log.Type}] {log.Tick}: {log.Message}";
        }
        else {
            return $"{log.LevelString()} [{log.Type}] {log.Tick}: {log.Message} x{log.DedupCount + 1}";
        }
    }
}