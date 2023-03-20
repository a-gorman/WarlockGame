namespace NeonShooter.Core.Game.Spell;

static class SpellFactory
{
    public static Fireball Fireball()
    {
        return new Fireball
        {
            ManaCost = 10, 
            CooldownTime = 1
        };
    }
}