using System;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class DebugVisualize : Behavior {
    public Color Color { get; set; } = Color.White;
    
    public override void Update(Entity entity) {
        switch (entity.CollisionType) {
            case CollisionType.None:
                SimDebug.VisualizePoint(entity.Position, Color);
                break;
            case CollisionType.Circle:
                SimDebug.VisualizeCircle(entity.Radius, entity.Position, Color);
                break;
            case CollisionType.Rectangle:
                SimDebug.Visualize(entity.BoundingRectangle, Color);
                break;
            case CollisionType.OrientedRectangle:
                SimDebug.Visualize(entity.OrientedRectangle, Color);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}