using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Perks;

class Perk {
    public int Id { get; set; }
    public PerkType Type { get; set; }
    public string Name { get; }
    public string Description { get; }
    public Texture2D Texture { get; }

    public Perk(PerkType type, string name, string description, Texture2D texture) {
        Name = name;
        Description = description;
        Texture = texture;
        Type = type;
    }
    public virtual void Update(Simulation sim) { }
    public virtual void OnAdded(int forceId, Simulation sim) { }
    public virtual void OnRemoved(int forceId, Simulation sim) { }
}