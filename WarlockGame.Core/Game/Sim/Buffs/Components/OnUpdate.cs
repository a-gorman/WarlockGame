
public class OnUpdate {
    private GameTimer _timer;

    private readonly SimTime _repetionRate;
    private readonly SpellContext _context;
    private readonly ISelfSpellComponent[] _components;

    public OnUpdate(SpellContext context, SimTime repeatEvery, ISelfSpellComponent[] components, boolean triggerOnApplication = false) {
        _context = context;
        _components = components;
        _repetionRate = repeatEvery;
        _timer = triggerOnApplication ? GameTimer.FromTicks(0) : repeatEvery.ToTimer();
    }

    public override void OnUpdate(Buff buff, Warlock target) {
        _timer = _timer.Decremented();
        if(!_timer.isExpired) {
            return;
        }

        foreach(var component in _components) {
            component.Invoke(component)
        }

        _timer = _repetionRate.ToTimer()
    }
}