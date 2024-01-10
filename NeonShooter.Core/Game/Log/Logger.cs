using System;
using System.Collections.Generic;

namespace NeonShooter.Core.Game.Log; 

/// <summary>
/// Barebones simple logger.
/// Will leak memory over time.
/// Read with debugger
/// </summary>
public static class Logger {
    private static readonly List<string> _logs = new List<string>();

    public static IReadOnlyList<string> Log => _logs;
    
    public static void Info(string log) {
        _logs.Add(String.Join(": ", "INFO", DateTime.Now.ToString("h:mm:ss.fff"), log));
    }
}