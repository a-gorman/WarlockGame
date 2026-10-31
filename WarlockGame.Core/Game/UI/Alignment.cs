using System;

namespace WarlockGame.Core.Game.UI;

enum Alignment {
    TopLeft       = AlignmentFlags.Left             | AlignmentFlags.Top,
    TopCenter     = AlignmentFlags.HorizontalCenter | AlignmentFlags.Top,
    TopRight      = AlignmentFlags.Right            | AlignmentFlags.Top,
    CenterLeft    = AlignmentFlags.Left             | AlignmentFlags.VerticalCenter,
    Center        = AlignmentFlags.HorizontalCenter | AlignmentFlags.VerticalCenter,
    CenterRight   = AlignmentFlags.Right            | AlignmentFlags.VerticalCenter,
    BottomLeft    = AlignmentFlags.Left             | AlignmentFlags.Bottom,
    BottomCenter  = AlignmentFlags.HorizontalCenter | AlignmentFlags.Bottom,
    BottomRight   = AlignmentFlags.Right            | AlignmentFlags.Bottom
}

[Flags]
enum AlignmentFlags {
    Left = 0b0001,
    Right = 0b0010,
    HorizontalCenter = 0b0100,
    Top = 0b1000,
    Bottom = 0b0001_0000,
    VerticalCenter = 0b0010_0000
}