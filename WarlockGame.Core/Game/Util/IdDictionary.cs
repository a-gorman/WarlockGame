using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WarlockGame.Core.Game.Util;

/// <summary>
/// A simple data structure for keeping track of items by integerId
/// Does not re-use keys
/// </summary>
class IdDictionary<T> : IReadOnlyDictionary<int, T> {
    private readonly Dictionary<int, T> _dict = new();
    public int NextValue { get; private set; } = 1;

    public Dictionary<int, T>.Enumerator GetEnumerator() {
        return _dict.GetEnumerator();
    }
    
    public void Clear() {
        _dict.Clear();
        NextValue = 1;
    }

    public int AddManual(T value) {
        var id = NextValue++;
        _dict.Add(id, value);
        return id;
    }
    
    public int AddNew(Func<int, T> valueCtor) {
        var id = NextValue++;
        _dict.Add(id, valueCtor.Invoke(id));
        return id;
    }
    
    public int AddExisting(T value, Action<int, T> idAssignment) {
        var id = NextValue++;
        idAssignment.Invoke(id, value);
        _dict.Add(id, value);
        return id;
    }
    
    public bool TryGetValue(int key, [MaybeNullWhen(false)] out T value) {
        return _dict.TryGetValue(key, out value);
    }

    public bool Remove(int key) {
        return  _dict.Remove(key);
    }
    
    public bool ContainsKey(int key) {
        return _dict.ContainsKey(key);
    }

    public T this[int key] {
        get => _dict[key];
        set => _dict[key] = value;
    }

    IEnumerable<int> IReadOnlyDictionary<int, T>.Keys => _dict.Keys;

    IEnumerable<T> IReadOnlyDictionary<int, T>.Values => _dict.Values;

    int IReadOnlyCollection<KeyValuePair<int, T>>.Count => _dict.Count;
    
    IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

    IEnumerator<KeyValuePair<int, T>> IEnumerable<KeyValuePair<int, T>>.GetEnumerator() => _dict.GetEnumerator();
}