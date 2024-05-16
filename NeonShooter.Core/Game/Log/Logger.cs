using System;
using System.Collections.Generic;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Log; 

/// <summary>
/// Barebones simple logger.
/// Will leak memory over time.
/// Read with debugger
/// </summary>
public static class Logger {
    private static readonly CircularBuffer<string> _logs = new CircularBuffer<string>(1000);

    public static IEnumerable<string> Log => _logs;
    
    public static void Info(string log) {
        _logs.PushFront(String.Join(": ", "INFO", DateTime.Now.ToString("h:mm:ss.fff"), log));
    }
    
    public static void Warning(string log) {
        _logs.PushFront(String.Join(": ", "WARNING", DateTime.Now.ToString("h:mm:ss.fff"), log));
    }
}