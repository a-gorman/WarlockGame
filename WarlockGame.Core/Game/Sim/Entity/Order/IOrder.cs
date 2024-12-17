namespace WarlockGame.Core.Game.Sim.Entity.Order; 

interface IOrder {
    
    /// <summary>
    /// Indicates if an order is finished and should be removed
    /// </summary>
    public bool Finished { get; }
    
    /// <summary>
    /// Updates the player character for the next tick.
    /// </summary>
    public void Update();

    /// <summary>
    /// Called when an order is canceled
    /// </summary>
    public void OnCancel();

    /// <summary>
    /// Called when an order is finished
    /// </summary>
    public void OnFinish();
}