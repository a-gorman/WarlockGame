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
    public bool Visible { get; set; } = false;

    private readonly TextDisplay _textDisplay;

    private LogDisplay() {
        _textDisplay = new TextDisplay
        {
            Bounds = new Rectangle(0,0, 900, 100)
        };

        Components = new[] { _textDisplay };
    }
    public IEnumerable<IInterfaceComponent> Components { get; }
    public void Draw(SpriteBatch spriteBatch) {}

    public void Refresh() {
        var logs = Logger.Logs.Select(x => String.Join(": ", x.LevelString(), x.Tick, x.Message)).Take(5).JoinToString('\n');
        _textDisplay.Text = logs;
    }
}