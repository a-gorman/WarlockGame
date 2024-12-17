using System;
using System.Linq;
using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.Sim.Entity.Factory; 

static class WarlockFactory {
    public static Warlock FromPacket(Networking.Packet.Warlock packet) {
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

    public static Networking.Packet.Warlock ToPacket(Warlock warlock) {
        return new Networking.Packet.Warlock {
            PlayerId = warlock.PlayerId,
            Position = warlock.Position,
            Velocity = warlock.Velocity,
            Orientation = warlock.Orientation,
            Health = warlock.Health,
            Spells = warlock.Spells.Select(x => new Networking.Packet.Spell { SpellId = x.SpellId, CooldownRemaining = x.Cooldown.FramesRemaining }).ToList()
        };
    }

    // TODO: Make dynamic
    private static WarlockSpell CreateSpell(Networking.Packet.Spell packet) {
        var spell = packet.SpellId switch
        {
            1 => SpellFactory.Fireball(),
            2 => SpellFactory.Lightning(),
            3 => SpellFactory.Poison(),
            4 => SpellFactory.Burst(),
            5 => SpellFactory.WindShield(),
            _ => throw new ArgumentException("Tried to deserialize unknown spell id")
        };

        spell.Cooldown.FramesRemaining = packet.CooldownRemaining;
        return spell;
    }
}