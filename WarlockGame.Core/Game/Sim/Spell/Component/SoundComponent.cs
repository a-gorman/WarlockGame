namespace WarlockGame.Core.Game.Sim.Spell.Component;

class SoundComponent : ILocationSpellComponent, ISelfSpellComponent {
    private readonly GameSound _sound;
    private readonly int? _radius;

    public SoundComponent(GameSound sound, int? radius = null) {
        _sound = sound;
        _radius = radius;
    }

    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        if (_radius == null) {
            _sound.Play(invokeLocation);
        } else {
            _sound.Play(invokeLocation, _radius.Value);
        }
    }

    public void Invoke(SpellContext context) {
        Invoke(context, context.CastFromPosition);
    }
}