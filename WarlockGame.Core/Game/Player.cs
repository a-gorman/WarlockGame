using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game;

class Player {
    public int Id { get; }

    public string Name { get; }

    public Color Color { get; }
    public bool IsActive { get; } = true;
    public bool IsLocal { get; }
    
    public Player(string name, int id, Color color, bool isLocal) {
        Name = name;
        Id = id;
        IsLocal = isLocal;
        Color = color;
    }
}