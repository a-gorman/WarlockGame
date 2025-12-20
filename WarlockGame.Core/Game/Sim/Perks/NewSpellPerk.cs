using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.Sim.Perks;

abstract class NewSpellPerk : Perk {
    private SpellDefinition SpellDefinition { get; }

    public NewSpellPerk(int id, SpellDefinition spell)
        : base(id, spell.Name, spell.Name, spell.SpellIcon) {
        SpellDefinition = spell;
    }

    public override void OnAdded(int forceId, Simulation sim) {
        sim.SpellManager.AddSpell(forceId, SpellDefinition.Id);
    }
}

class HomingSpellPerk : NewSpellPerk {
    public HomingSpellPerk(SpellFactory spellFactory) : base(6, spellFactory.Homing()) { }
}