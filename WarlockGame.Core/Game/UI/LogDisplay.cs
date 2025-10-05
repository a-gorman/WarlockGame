using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.UI.Components;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

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

    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (IsDirty) {
            Refresh();
            IsDirty = false;
        }
    }

    public void Refresh() {
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
        if (log.DedupCount == 0)
        {
            return string.Join(": ", log.LevelString(), log.Tick, log.Message);
        }
        else
        {
            return string.Join(": ", log.LevelString(), log.Tick, $"{log.Message} x{log.DedupCount + 1}");
        }
    }
}