using System;
using System.Collections.Generic;
using System.Linq;

namespace WarlockGame.Core.Game.Util; 

public static class LinqExtensions {
    
    /// <summary>
    /// Performs on action on each element of a collection
    /// </summary>
    /// <param name="source">The source enumerable</param>
    /// <param name="action">The action to perform on each element</param>
    /// <typeparam name="T">The type of element</typeparam>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (var item in source) {
            action(item);
        }
    }
    
    /// <summary>
    /// Performs on action on each element of a collection, and returns the collection.
    /// </summary>
    /// <remarks>Similar to ForEach, but allows method chaining</remarks>
    /// <param name="source">The source enumerable</param>
    /// <param name="action">The action to perform on each element</param>
    /// <typeparam name="T">The type of element</typeparam>
    /// <returns>The unmodified source enumerable</returns>
    public static IEnumerable<T> OnEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (var item in source) {
            action(item);
        }

        return source;
    }

    /// <summary>
    /// Returns the given object
    /// </summary>
    /// <remarks>Convenience method for "x => x"</remarks>
    public static T Identity<T>(T self) => self;

    public static string JoinToString(this IEnumerable<string> stringCollection, string separator = ", ") {
        return string.Join(separator, stringCollection);
    }
    
    public static string JoinToString(this IEnumerable<string> stringCollection, char separator) {
        return string.Join(separator, stringCollection);
    }

    public static TOut? MaxOrDefault<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> selector, TOut? defaultValue = default) {
        return source.Any() ? source.Max(selector) : defaultValue;
    }
    
    public static TSource? MaxOrDefault<TSource>(this IEnumerable<TSource> source, TSource? defaultValue = default) {
        return source.Any() ? source.Max() : defaultValue;
    }
    
    public static TOut? MinOrDefault<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> selector, TOut? defaultValue = default) {
        return source.Any() ? source.Min(selector) : defaultValue;
    }
    
    public static TSource? MinOrDefault<TSource>(this IEnumerable<TSource> source, TSource? defaultValue = default) {
        return source.Any() ? source.Min() : defaultValue;
    }
    
    public static IEnumerable<T> Reverse<T>(this LinkedList<T> list) {
        var el = list.Last;
        while (el != null) {
            yield return el.Value;
            el = el.Previous;
        }
    }
}