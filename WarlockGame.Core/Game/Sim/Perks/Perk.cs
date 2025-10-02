using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Perks;

class Perk {
    public string Name { get; }
    public string Description { get; }
    public Texture2D Texture { get; }

    public Perk(PerkType type, string name, string description, Texture2D texture) {
        Name = name;
        Description = description;
        Texture = texture;
        Type = type;
    }
    public int Id { get; set; }
    public PerkType Type { get; set; }
    public virtual void Update(Simulation sim) { }
    public virtual void OnChosen(int forceId, Simulation sim) { }
    public virtual void OnPerkRemoved(int forceId, Simulation sim) { }
}