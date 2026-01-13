using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Sim.Perks;

class Perk {
    public int Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Texture2D Texture { get; }

    public Perk(int id, string name, string description, Texture2D texture) {
        Name = name;
        Description = description;
        Texture = texture;
        Id = id;
    }
    
    public virtual void Update(Simulation sim) { }
    public virtual void OnAdded(int forceId, Simulation sim) { }
    public virtual void OnRemoved(int forceId, Simulation sim) { }
    public virtual void Clear(Simulation sim) { }
}