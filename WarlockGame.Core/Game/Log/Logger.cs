using System;
using System.Collections.Generic;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Log; 

/// <summary>
/// Barebones simple logger.
/// Will leak memory over time.
/// Read with debugger
/// </summary>
public static class Logger {
    private static readonly CircularBuffer<Log> _logs = new CircularBuffer<Log>(1000);

    public static IEnumerable<Log> Logs => _logs;
    
    public static void Info(string message) {
        WriteLog(message, Level.INFO);
    }
    
    public static void Warning(string message) {
        WriteLog(message, Level.WARNING);
    }

    public static void WriteLog(string message, Level level) {
        if (!_logs.IsEmpty && _logs.Front().Message == message) {
            return;
        }
        
        _logs.PushFront(new Log
        {
            Level = level,
            Message = message,
            Tick = Simulation.Instance.Tick,
            Timestamp = DateTime.Now
        });
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
                Level.INFO => "INFO",
                Level.DEBUG => "DEBUG",
                Level.WARNING => "WARN",
                Level.ERROR => "ERROR",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public enum Level {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }
}