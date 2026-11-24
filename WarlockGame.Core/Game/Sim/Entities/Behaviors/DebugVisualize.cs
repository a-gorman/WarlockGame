using System;

namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class DebugVisualize : Behavior {
    public Color Color { get; set; } = Color.White;
    
    public override void Update(Entity entity) {
        switch (entity.CollisionShape) {
            case CollisionShape.None:
                SimDebug.VisualizePoint(entity.Position, Color);
                break;
            case CollisionShape.Circle:
                SimDebug.VisualizeCircle(entity.Radius, entity.Position, Color);
                break;
            case CollisionShape.Rectangle:
                SimDebug.Visualize(entity.BoundingRectangle, Color);
                break;
            case CollisionShape.OrientedRectangle:
                SimDebug.Visualize(entity.OrientedRectangle, Color);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}