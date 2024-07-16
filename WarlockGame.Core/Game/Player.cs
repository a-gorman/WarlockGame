using Warlock = WarlockGame.Core.Game.Entity.Warlock;

namespace WarlockGame.Core.Game;

class Player {
    public int Id { get; }

    public string Name { get; }

    public bool IsActive { get; } = true;
    public bool IsLocal { get; }
    
    public Player(string name, int id, bool isLocal) {
        Name = name;
        Id = id;
        IsLocal = isLocal;
    }

    public void Update() {
    }
}