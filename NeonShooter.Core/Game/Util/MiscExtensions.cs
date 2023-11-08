using System;

namespace NeonShooter.Core.Game.Util; 

static class MiscExtensions {

    public static TOut Let<TIn, TOut>(this TIn source, Func<TIn, TOut> func) => func(source);
    
    public static T Also<T>(this T source, Action<T> action) {
        action(source);
        return source;
    }

    public static T Apply<T>(this T source, Action<T> action) {
        action(source);
        return source;
    }

    public static void Run<T>(this T source, Action<T> action) => action(source);
}