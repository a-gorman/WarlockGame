namespace WarlockGame.Core.Game.Sim.Spell.AreaOfEffect;

interface IDirectionalShape {
    AoeResult GatherTargets(SpellContext context, Vector2 castLocation, Vector2 invokeDirection);
}

interface ILocationShape {
    AoeResult GatherTargets(SpellContext context, Vector2 invokeLocation);
}