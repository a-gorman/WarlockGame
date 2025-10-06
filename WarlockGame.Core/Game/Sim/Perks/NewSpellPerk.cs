using WarlockGame.Core.Game.Sim.Spell;

namespace WarlockGame.Core.Game.Sim.Perks;

class NewSpellPerk: Perk {
    private SpellDefinition SpellDefinition { get; }

    public NewSpellPerk(SpellDefinition spell) 
        : base(PerkType.NewSpells, spell.Name, spell.Name, spell.SpellIcon) {
        SpellDefinition = spell;
    }

    public override void OnAdded(int forceId, Simulation sim) {
        sim.SpellManager.AddSpell(forceId, SpellDefinition.Id);
    }
}