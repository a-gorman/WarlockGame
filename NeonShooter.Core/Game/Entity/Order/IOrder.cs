namespace NeonShooter.Core.Game.Entity.Order; 

interface IOrder {
    
    /// <summary>
    /// Indicates if an order is finished and should be removed
    /// </summary>
    public bool Finished { get; }
    
    /// <summary>
    /// Updates the player character for the next tick.
    /// Ends once the player has reached the destination
    /// </summary>
    /// <returns>True if the order is complete</returns>
    public bool Update();

    /// <summary>
    /// Called when an order is canceled
    /// </summary>
    public void OnCancel();

    /// <summary>
    /// Called when an order is finished
    /// </summary>
    public void OnFinish();
}