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

    private List<string> _wrappedText = null!;
    private bool _isTextDirty;
    
    private readonly StringBuilder _sb = new();

    public TextDisplay(string text = "", SpriteFont? font = null) {
        Text = text;
        Font = font ?? Art.Font;
        Clickable = ClickableState.Ignore;
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (BackgroundColor != null) {
            spriteBatch.Draw(Art.Pixel, BoundingBox.WithOffset(location), BackgroundColor.Value);
        }

        if(IsLayoutDirty || _isTextDirty) {
            RecalculateWrappedText();
        }

        for (var i = 0; i < _wrappedText.Count; i++) {
            var line = _wrappedText[i];
            spriteBatch.DrawString(Art.Font, line, 
                position: BoundingBox.Location.ToVector2().Translate(0, Font.LineSpacing * TextScale * i) + location, 
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

        _wrappedText = TextUtil.WrapText(Text, TextMeasurement, BoundingBox.Width, maxHeight: BoundingBox.Height, truncation: TruncationCharacters);
    }
}