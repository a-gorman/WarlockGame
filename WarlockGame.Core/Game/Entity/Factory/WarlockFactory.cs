using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Spell;

namespace WarlockGame.Core.Game.Entity.Factory; 

static class WarlockFactory {
    public static Warlock FromPacket(Networking.Warlock packet) {
        var warlock = new Warlock(packet.PlayerId)
        {
            Position = packet.Position,
            Velocity = packet.Velocity,
            Orientation = packet.Orientation,
            Health = packet.Health
        };
        
        warlock.Spells.Clear();
        warlock.Spells.AddRange(packet.Spells.Select(CreateSpell));
        return warlock;
    }

    public static Networking.Warlock ToPacket(Warlock warlock) {
        return new Networking.Warlock {
            PlayerId = warlock.PlayerId,
            Position = warlock.Position,
            Velocity = warlock.Velocity,
            Orientation = warlock.Orientation,
            Spells = warlock.Spells.Select(x => new Networking.Spell { SpellId = x.SpellId, CooldownRemaining = x.Cooldown.FramesRemaining }).ToList()
        };
    }

    private static WarlockSpell CreateSpell(Networking.Spell packet) {
        var spell = packet.SpellId switch
        {
            1 => SpellFactory.Fireball(),
            2 => SpellFactory.Lightning(),
            _ => throw new ArgumentException("Tried to deserialize unknown spell id")
        };

        spell.Cooldown.FramesRemaining = packet.CooldownRemaining;
        return spell;
    }
}