using MonoGame.Extended;

namespace WarlockGame.Core.Game.Util;

public struct RotatedRectangle {
    public Angle Rotation { get; }
    public BoundingRectangle Rectangle { get; }
}