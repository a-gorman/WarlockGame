using System;
using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game.Spell;

namespace NeonShooter.Core.Game.Entity.Factory; 

static class WarlockFactory {
    public static Warlock FromPacket(Networking.Warlock packet, int playerId) {
        var warlock = new Warlock(packet.Id, playerId)
        {
            Position = packet.Position,
            Velocity = packet.Velocity,
            Orientation = packet.Orientation,
        };
        
        warlock.Spells.Clear();
        warlock.Spells.AddRange(CreateSpells(packet));
        return warlock;
    }

    public static Networking.Warlock ToPacket(Warlock warlock) {
        return new Networking.Warlock {
            Id = warlock.Id,
            Position = warlock.Position,
            Velocity = warlock.Velocity,
            Orientation = warlock.Orientation,
            SpellIds = warlock.Spells.Select(x => x.SpellId).ToArray(),
            SpellCooldowns = warlock.Spells.Select(x => x.Cooldown.FramesRemaining).ToArray(),
        };
    }

    private static IEnumerable<WarlockSpell> CreateSpells(Networking.Warlock warlock) {
        for (var index = 0; index < warlock.SpellIds.Length; index++) {
            var spell = warlock.SpellIds[index] switch
            {
                1 => SpellFactory.Fireball(),
                2 => SpellFactory.Lightning(),
                _ => throw new ArgumentOutOfRangeException("spellId")
            };

            spell.Cooldown.FramesRemaining = warlock.SpellCooldowns[index];
            yield return spell;
        }
    }
}