using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Log; 

public static class Logger {
    private static readonly CircularBuffer<Log> _logs = new(1000);

    public static IEnumerable<Log> Logs => _logs;

    public static event Action<Log>? LogCreated;

    // Deduplicates sequential logs with the same message at this level or below
    public static Level DedupeLevel { get; set; } = Level.ERROR;
    
    public static void Debug(string message, LogType logType) {
        WriteLog(message, Level.DEBUG, logType);
    }
    
    public static void Info(string message, LogType logType) {
        WriteLog(message, Level.INFO, logType);
    }
    
    public static void Warning(string message, LogType logType) {
        WriteLog(message, Level.WARNING, logType);
    }
    
    public static void Error(string message, LogType logType) {
        WriteLog(message, Level.ERROR, logType);
    }

    public static void WriteLog(string message, Level level, LogType logType) {
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
                Tick = WarlockGame.Instance?.Simulation.Tick ?? 0,
                Type = logType,
                Timestamp = DateTime.Now
            });
        }

        LogCreated?.Invoke(_logs.Front());
    }

    public class Log {
        public required string Message { get; init; }
        public required DateTime Timestamp { get; set; }
        public required int Tick { get; set; }
        public required Level Level { get; init; }
        public required LogType Type { get; init; }
        public int DedupCount { get; set; } = 0;

        public string LevelString() {
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

    [Flags]
    public enum LogType {
        None = 0,
        Interface = 1,
        PlayerAction = 2,
        Network = 4,
        Simulation = 8,
        Program = 16,
        All = Interface | PlayerAction | Network | Simulation | Program
    }
}