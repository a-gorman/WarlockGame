using System.Collections.Generic;
using System.Linq;
using NeonShooter.Core.Game.Action;

namespace NeonShooter.Core.Game.Networking; 

public class RemotePlayerInput {
    public required int PlayerId { get; init; }

    public IEnumerable<IGameAction> GetGameActions(int frame) {
        return Enumerable.Empty<IGameAction>();
    }
    
    
}