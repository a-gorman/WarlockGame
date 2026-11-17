namespace WarlockGame.Core.Game.Sim.Spell.Component;

class SoundComponent(GameSound sound, int? radius = null): ILocationSpellComponent {
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        if (radius == null) {
            sound.Play(invokeLocation);
        } else {
            sound.Play(invokeLocation, radius.Value);
        }
    }
}