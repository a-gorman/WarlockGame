using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.UI;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Log; 

public static class Logger {
    private static readonly CircularBuffer<Log> _logs = new(1000);

    public static IEnumerable<Log> Logs => _logs;
    
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
        if (!_logs.IsEmpty && _logs.Front().Message == message) {
            return;
        }
        
        _logs.PushFront(new Log
        {
            Level = level,
            Message = message,
            Tick = WarlockGame.Instance.Simulation.Tick,
            Timestamp = DateTime.Now
        });

        LogDisplay.Instance.Refresh();
    }

    public class Log {
        public required String Message { get; init; }
        public required DateTime Timestamp { get; init; }
        public required int Tick { get; init; }
        public required Level Level { get; init; }
        public int DedupCount { get; set; } = 1;

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
        DEBUG = 0,
        INFO = 1,
        WARNING = 2,
        ERROR = 3
    }
}