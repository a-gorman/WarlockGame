namespace NeonShooter.Core.Game.Entity.Order; 

interface IOrder {
    /// <summary>
    /// Updates the player character for the next tick.
    /// Ends once the player has reached the destination
    /// </summary>
    /// <param name="player">The player to update</param>
    /// <returns>True if the order is complete</returns>
    public bool Update(PlayerShip player);
}