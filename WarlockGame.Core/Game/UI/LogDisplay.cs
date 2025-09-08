using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

class LogDisplay : InterfaceComponent {

    public static LogDisplay Instance { get; } = new LogDisplay();

    public Logger.Level DisplayLevel {
        get;
        set {
            field = value;
            Refresh();
        }
    } = Logger.Level.INFO;

    public override bool Visible {
        get;
        set {
            field = value;
            if (value) {
                Refresh();
            }
        }
    } = false;

    private readonly TextDisplay _textDisplay;

    private LogDisplay() {
        _textDisplay = new TextDisplay
        {
            Bounds = new Rectangle(0,0, 900, 100)
        };

        AddComponent(_textDisplay);
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