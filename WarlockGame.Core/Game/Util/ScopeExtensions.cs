using System;
using System.Runtime.CompilerServices;

namespace WarlockGame.Core.Game.Util; 

/// <summary>
/// Kotlin-like scope extensions
/// </summary>
static class ScopeExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut Let<TIn, TOut>(this TIn source, Func<TIn, TOut> func) => func(source);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Also<T>(this T source, Action<T> action) {
        action(source);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Apply<T>(this T source, Action<T> action) {
        action(source);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Run<T>(this T source, Action<T> action) => action(source);
}