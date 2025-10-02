using System.Collections.Generic;
using System.Linq;

namespace WarlockGame.Core.Game.Sim.Perks;

// class NewSpellPerk: Perk {
//     private readonly int[] _spellTypeIds;
//
//     public NewSpellPerk(IEnumerable<int> spellTypeIds) : base(PerkType.NewSpells) {
//         _spellTypeIds = spellTypeIds.ToArray();
//     }
//
//     public override void OnChosen(int forceId, Simulation sim) {
//         foreach (var spellTypeId in _spellTypeIds) {
//             sim.SpellManager.AddSpell(forceId, spellTypeId);
//         }
//     }
// }