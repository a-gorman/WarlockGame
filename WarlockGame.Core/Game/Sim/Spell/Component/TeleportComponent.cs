namespace WarlockGame.Core.Game.Sim.Spell.Component;

class TeleportComponent: ILocationSpellComponent {
    public void Invoke(SpellContext context, Vector2 invokeLocation) {
        context.Caster.Position = invokeLocation;
    }
}