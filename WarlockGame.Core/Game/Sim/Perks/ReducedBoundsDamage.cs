using WarlockGame.Core.Game.Graphics;

namespace WarlockGame.Core.Game.Sim.Perks;

class ReducedBoundsDamage: Perk {
    private const float Factor = 0.1f;


    public ReducedBoundsDamage() 
        : base(
            id: 5,            
            name: "Reduced Bounds Damage",
            description: "Permanently reduces the damage you take from being out of bounds.",
            texture: Art.ReducedBoundaryDamageIcon) {
        
    }
}