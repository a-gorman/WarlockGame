namespace WarlockGame.Core.Game.Sim.Spell.Component;

class SoundComponent(GameSound sound): ILocationSpellComponent {
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        sound.Play(invokeLocation);
    }
}