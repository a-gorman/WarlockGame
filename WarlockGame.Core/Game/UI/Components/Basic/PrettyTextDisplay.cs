using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class PrettyTextDisplay : InterfaceComponent {
    public PrettyString Text {
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
    
    private readonly List<Line> _lines = [];
    private bool _isTextDirty;
    
    public PrettyTextDisplay(PrettyString text, Alignment textAlignment = Alignment.TopLeft, SpriteFont? font = null) {
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

        // Problem: We need the width of each individual token
        var textColor = TextColor;
        foreach (var line in _lines) {
            var currentWidth = 0f;
            foreach (var token in line.Tokens) {
                switch (token.Token) {
                    case string text:
                        spriteBatch.DrawString(Font, text,
                            position: BoundingBox.Location.ToVector2() + location + new Vector2(currentWidth, 0) + line.Position,
                            color: textColor,
                            scale: TextScale,
                            rotation: 0,
                            origin: Vector2.Zero,
                            effects: SpriteEffects.None,
                            layerDepth: 0);
                        break;
                    case PrettyString.Formatting formatting:
                        textColor = formatting.Color;
                        break;
                }
                currentWidth += token.Width;
            }

        }
    }
    
    private void RecalculateWrappedText() {
        Vector2 TextMeasurement(ReadOnlySpan<char> x) {
            return Font.MeasureString(x) * TextScale;
        }

        _lines.Clear();
        var currentTokens = new List<Line.LineToken>();
        var currentWidth = 0f;
        foreach (var token in Text.Tokens) {
            switch (token) {
                case string textToken: {
                    var rawLines = TextUtil.WrapText(textToken, 
                        TextMeasurement, 
                        BoundingBox.Width, 
                        startingIndent: currentWidth, 
                        maxHeight: BoundingBox.Height, 
                        truncator: "");
                    
                    var firstNewLine = rawLines[0];
                    // Join first new line with last old line
                    var widthDelta = firstNewLine.Width - currentWidth;
                    currentWidth = firstNewLine.Width;
                    if (currentTokens.Count > 1 && currentTokens[^1].Token is string stringToJoin) {
                        currentTokens[^1] = new Line.LineToken(stringToJoin + firstNewLine.Text, widthDelta + currentTokens[^1].Width);
                    } else {
                        currentTokens.Add(new Line.LineToken(firstNewLine.Text, widthDelta));
                    }
                    
                    if (rawLines.Count > 1) {
                        // Close out the current line and add all the lines before the last line
                        _lines.Add(new Line(currentTokens.ToArray()));
                        foreach (var line in rawLines.Skip(1)) {
                            _lines.Add(new Line([new Line.LineToken(line.Text, line.Width)]));
                        }
                        // Make the last line the current line
                        currentTokens.Clear();
                        currentTokens.Add(new Line.LineToken(rawLines[^1].Text, rawLines[^1].Width));
                        currentWidth = rawLines[^1].Width;
                    }
                    break;
                }
                case PrettyString.Formatting formatting:
                    if (currentTokens.Count > 1 && currentTokens[^1].Token is PrettyString.Formatting) {
                        currentTokens[^1] = new Line.LineToken(formatting, 0);
                    } else {
                        currentTokens.Add(new Line.LineToken(formatting, 0));
                    }
                    break;
            }
        }
        // Close out the last dangling line
        _lines.Add(new Line(currentTokens.ToArray()));
            
        var lineSpacing = Font.LineSpacing * TextScale;
        var lineCount = _lines.Count;
        
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
                    xOffset = 0;
                    break;
                case Alignment.TopCenter:
                case Alignment.Center:
                case Alignment.BottomCenter:
                    xOffset = Math.Abs(BoundingBox.Width - _lines[i].Tokens.Sum(x => x.Width)) / 2;
                    break;
                case Alignment.TopRight:
                case Alignment.CenterRight:
                case Alignment.BottomRight:
                    xOffset = Math.Abs(BoundingBox.Width - _lines[i].Tokens.Sum(x => x.Width));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(TextAlignment), TextAlignment, null);
            }

            _lines[i] = _lines[i] with { Position = new Vector2(xOffset, yOffset)};
        }
    }

    private struct Line {
        public struct LineToken {
            public LineToken(PrettyString.Token token, float width) {
                Token = token;
                Width = width;
            }
            
            public PrettyString.Token Token { get; }
            public float Width { get; }
        }
        public LineToken[] Tokens { get; }
        public Vector2 Position { get; init; }
        
        public Line(LineToken[] tokens) {
            Tokens = tokens;
            Position = new();
        }
    }
}