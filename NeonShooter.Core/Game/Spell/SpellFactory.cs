namespace NeonShooter.Core.Game.Spell;

static class SpellFactory
{
    public static Spell Fireball()
    {
        return new Spell
        {
            ManaCost = 10, 
            CooldownTime = 1,
            SpellIcon = Art.FireballIcon
        };
    }
}