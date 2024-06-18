namespace WarlockGame.Core.Game.Action; 

/// <summary>
/// A player command that affects game state, such as a move order
/// </summary>
public interface IGameAction {
    int TargetFrame { get; }
    
    void Execute();
}