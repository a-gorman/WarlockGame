using System;
using System.Collections.Generic;

namespace WarlockGame.Core.Game.Sim;

class EventManager {

    private readonly UnkeyedArglessEvent _simRestarted = new();

    public void SubscribeToSimRestarted(Action action) {
        _simRestarted.Subscriptions.Add(action);
    }
    
    public void UnsubscribeToSimRestarted(Action action) {
        _simRestarted.Subscriptions.Add(action);
    }
    
    public void OnSimRestarted() {
        foreach (var subscription in _simRestarted.Subscriptions) {
            subscription.Invoke();
        }
    }
    
    private class KeyedEvent<TKey, TEvent> where TKey: notnull where TEvent: notnull {
        public List<Action<TEvent>> UnkeyedSubscriptions { get; } = [];
        public Dictionary<TKey, Action<TKey, TEvent>> KeyedSubscriptions { get; } = new Dictionary<TKey, Action<TKey, TEvent>>();
    }
    
    private class UnkeyedEvent<T> where T: notnull {
        public List<Action<T>> Subscriptions { get; } = [];
    }
    
    private class UnkeyedArglessEvent {
        public HashSet<Action> Subscriptions { get; } = [];
    }
}