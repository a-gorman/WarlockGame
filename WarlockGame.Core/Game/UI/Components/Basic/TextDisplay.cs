using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class TextDisplay : InterfaceComponent {
    public string Text {
        get;
        set {
            field = value;
            _isTextDirty = true;
        }
    }

    public Color? BackgroundColor { get; set; } = null;
    public Color TextColor { get; set; } = Color.White;
    public float TextScale { get; set; } = 1f;

    public string TruncationCharacters {
        get;
        set {
            field = value;
            _isTextDirty = true;
        }
    } = "";

    public SpriteFont Font {
        get;
        set {
            field = value;
            _isTextDirty = true;
        }
    }

    public Alignment TextAlignment {
        get;
        set {
            field = value;
            _isTextDirty = true;
        }
    }
    
    private Line[] _lines = null!;
    private bool _isTextDirty;
    
    private readonly StringBuilder _sb = new();

    public TextDisplay(string text = "", Alignment textAlignment = Alignment.TopLeft, SpriteFont? font = null) {
        Text = text;
        Font = font ?? Art.Font;
        Clickable = ClickableState.Ignore;
        TextAlignment = textAlignment;
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) { 
        if (BackgroundColor != null) {
            spriteBatch.Draw(Art.Pixel, BoundingBox.WithOffset(location), BackgroundColor.Value);
        }

        if (IsLayoutDirty || _isTextDirty) {
            RecalculateWrappedText();
        }

        for (var i = 0; i < _lines.Length; i++) {
            spriteBatch.DrawString(Art.Font, _lines[i].Text,
                position: BoundingBox.Location.ToVector2() + location + _lines[i].Position,
                color: TextColor,
                scale: TextScale,
                rotation: 0,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
    
    // Thread unsafe
    private void RecalculateWrappedText() {
        Vector2 TextMeasurement(ReadOnlySpan<char> x) {
            _sb.Clear();
            return Font.MeasureString(_sb.Append(x)) * TextScale;
        }

        var unprocessedLines = TextUtil.WrapText(Text, TextMeasurement, BoundingBox.Width, maxHeight: BoundingBox.Height, truncator: TruncationCharacters);
        _lines = new Line[unprocessedLines.Count];

        var lineSpacing = Font.LineSpacing * TextScale;
        var lineCount = unprocessedLines.Count;
        
        for (var i = 0; i < lineCount; i++) {
            float yOffset;
            switch (TextAlignment) {
                case Alignment.TopLeft:
                case Alignment.TopCenter:
                case Alignment.TopRight:
                    yOffset = i * lineSpacing;
                    break;
                case Alignment.CenterLeft:
                case Alignment.Center:
                case Alignment.CenterRight:
                    yOffset = (float)BoundingBox.Height / 2 + (i - (float)lineCount / 2) * lineSpacing;
                    break;
                case Alignment.BottomLeft:
                case Alignment.BottomCenter:
                case Alignment.BottomRight:
                    yOffset = BoundingBox.Height - (lineCount - i) * lineSpacing;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(TextAlignment), TextAlignment, null);
            }

            float xOffset;
            switch (TextAlignment) {
                case Alignment.TopLeft:
                case Alignment.CenterLeft:
                case Alignment.BottomLeft:
                    xOffset = i * lineSpacing;
                    break;
                case Alignment.TopCenter:
                case Alignment.Center:
                case Alignment.BottomCenter:
                    xOffset = Math.Abs(BoundingBox.Width - unprocessedLines[i].Width) / 2;
                    break;
                case Alignment.TopRight:
                case Alignment.CenterRight:
                case Alignment.BottomRight:
                    xOffset = Math.Abs(BoundingBox.Width - unprocessedLines[i].Width);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(TextAlignment), TextAlignment, null);
            }

            _lines[i] = new Line(unprocessedLines[i].Text, new Vector2(xOffset, yOffset));
        }
    }

    private struct Line {
        public string Text { get; }
        public Vector2 Position { get; }
        
        public Line(string text, Vector2 position) {
            Text = text;
            Position = position;
        }
    }
}