namespace WarlockGame.Core.Game;

class Player {
    public int Id { get; }
    public int? ForceId { get; set; }

    public string Name { get; }

    public Color Color { get; }
    public bool IsActive { get; set; } = true;
    public bool IsLocal { get; }
    
    public Player(string name, int id, Color color, bool isLocal, int? forceId) {
        Name = name;
        Id = id;
        IsLocal = isLocal;
        ForceId = forceId;
        Color = color;
    }
}