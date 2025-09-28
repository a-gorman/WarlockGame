using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Log; 

public static class Logger {
    private static readonly CircularBuffer<Log> _logs = new(1000);

    public static IEnumerable<Log> Logs => _logs;

    // Deduplicates sequential logs with the same message at this level or below
    public static Level DedupeLevel { get; set; } = Level.ERROR;
    
    public static void Debug(string message) {
        WriteLog(message, Level.DEBUG);
    }
    
    public static void Info(string message) {
        WriteLog(message, Level.INFO);
    }
    
    public static void Warning(string message) {
        WriteLog(message, Level.WARNING);
    }
    
    public static void Error(string message) {
        WriteLog(message, Level.ERROR);
    }

    public static void WriteLog(string message, Level level) {
        if (!_logs.IsEmpty && level <= DedupeLevel && _logs.Front().Message == message) {
             _logs.Front().Apply(x =>
             {
                 x.Tick = WarlockGame.Instance.Simulation.Tick;
                 x.Timestamp = DateTime.Now;
                 x.DedupCount++;
             });
        }
        else
        {
            _logs.PushFront(new Log
            {
                Level = level,
                Message = message,
                Tick = WarlockGame.Instance.Simulation.Tick,
                Timestamp = DateTime.Now
            });
        }

        LogDisplay.Instance?.Refresh();
    }

    public class Log {
        public required String Message { get; init; }
        public required DateTime Timestamp { get; set; }
        public required int Tick { get; set; }
        public required Level Level { get; init; }
        public int DedupCount { get; set; } = 0;

        public String LevelString() {
            return Level switch
            {
                Level.DEBUG => "DEBUG",
                Level.INFO => "INFO",
                Level.WARNING => "WARN",
                Level.ERROR => "ERROR",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public enum Level {
        NONE = -1,
        DEBUG = 0,
        INFO = 1,
        WARNING = 2,
        ERROR = 3,
        FATAL = 4
    }
}