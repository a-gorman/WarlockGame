

class EventTriggeredComponent<T where T: Action> {

    public T? _action = null;
    public readonly Func<SpellContext, T> _actionSelector;

    public EventTriggeredComponent(Func<SpellContext, T> action, SimTime maxTime) {
        _actionSelector = action;
    }

    public void Invoke(SpellContext context) {
        
    }
}