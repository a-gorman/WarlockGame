using System.Collections.Generic;

namespace NeonShooter.Core.Game.Spell;

static class SpellFactory
{
    public static WarlockSpell Fireball()
    {
        return new WarlockSpell
        {
            ManaCost = 10, 
            CooldownTime = 1,
            SpellIcon = Art.FireballIcon,
            Effects = new List<ICastEffect>
            {
                new FireballEffect()
            }
        };
    }
    
    public static WarlockSpell Lightning()
    {
        return new WarlockSpell
        {
            ManaCost = 10, 
            CooldownTime = 1,
            SpellIcon = Art.LightningIcon,
            Effects = new List<ICastEffect>
            {
                new LightningEffect()
            }
        };
    }
}