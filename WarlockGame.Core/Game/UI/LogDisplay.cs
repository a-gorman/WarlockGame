using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.UI.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

class LogDisplay : IInterfaceComponent {

    public static LogDisplay Instance { get; } = new LogDisplay();
    
    public int Layer { get; set; }
    public bool IsExpired { get; set; }

    public Logger.Level DisplayLevel {
        get;
        set {
            field = value;
            Refresh();
        }
    } = Logger.Level.INFO;

    public bool Visible {
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

        Components = [_textDisplay];
    }
    public IEnumerable<IInterfaceComponent> Components { get; }
    public void Draw(SpriteBatch spriteBatch) {}

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