using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

sealed class LogDisplay : InterfaceComponent {
    public static LogDisplay Instance { get; } = new LogDisplay();

    private readonly Queue<Logger.Log> _displayedLogs = new();

    // Log types are bit flags. Each type is on or off at the given level given by its flag bit at that level.
    private readonly Logger.LogType[] _logTypeDisplayLevels = new Logger.LogType[5];

    private const int MaxDisplayedLogs = 5;

    private readonly TextDisplay _textDisplay;
    
    private LogDisplay() {
        BoundingBox = new Rectangle(0, 0, 900, 100);
        _textDisplay = new TextDisplay { TextScale = 0.6f };

        AddComponent(_textDisplay);
        SetDisplayLevel(Logger.Level.INFO);
    }

    public void SetDisplayLevel(Logger.Level level) {
        SetDisplayLevelForTypes(Logger.LogType.All, level);
    }
    
    /// <summary>
    /// Sets the log level for specific log types (since log type is a flag enum)
    /// </summary>
    public void SetDisplayLevelForTypes(Logger.LogType logTypes, Logger.Level level) {
        if (level == Logger.Level.NONE) level = (Logger.Level)5;
        
        for (var i = 0; i < _logTypeDisplayLevels.Length; i++) {
            if (i >= (int)level) {
                _logTypeDisplayLevels[i] |= logTypes;
            }
            else {
                _logTypeDisplayLevels[i] &= ~logTypes;
            }
        }

        IsDirty = true;
    }

    public override void OnAdd() {
        Logger.LogCreated += OnLogAdded;
        IsDirty = true;
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

    private void OnLogAdded(Logger.Log log) {
        if (ShouldDisplay(log)) {
            if (_displayedLogs.Count == MaxDisplayedLogs) {
                _displayedLogs.Dequeue();
            }
            
            _displayedLogs.Enqueue(log);
            RecalculateDisplayText();
        }
    }
    
    private void Refresh() {
        if (!Visible) return;
        
        _displayedLogs.Clear();
        foreach (var log in Logger.Logs.Where(ShouldDisplay).Take(MaxDisplayedLogs))
        {
            _displayedLogs.Enqueue(log);
        }

        RecalculateDisplayText();
    }

    private void RecalculateDisplayText() {
        _textDisplay.Text = _displayedLogs.Select(FormatLog).JoinToString('\n');
    }

    private bool ShouldDisplay(Logger.Log log) {
        // Include takes precedence over exclude
        return (log.Type & _logTypeDisplayLevels[(int)log.Level]) != 0; 
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