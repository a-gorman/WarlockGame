using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Basic;

class TextDisplay : InterfaceComponent {
    public required Rectangle Bounds {
        get;
        set {
            field = value;
            RecalculateWrappedText();
        }
    }

    public string Text {
        get => _text;
        set {
            _text = value;
            RecalculateWrappedText();
        }
    }
    private string _text = string.Empty;
    private string _wrappedText = string.Empty;

    public float TextScale { get; set; } = 1f;

    public SpriteFont Font {
        get;
        set {
            field = value;
            _fontHeight = value.MeasureString(" ").Y;
            RecalculateWrappedText();
        }
    }

    private float _fontHeight;

    public Color TextColor { get; set; } = Color.White;

    public TextDisplay(SpriteFont? font = null) {
        Font = font ?? Art.Font;
    }
    
    public override void Draw(SpriteBatch spriteBatch) {
        spriteBatch.DrawString(Art.Font, _wrappedText, Bounds.Location.ToVector2(), TextColor, scale: TextScale, rotation: 0, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0);
    }

    private void RecalculateWrappedText() {
        var sb = new StringBuilder();
        Vector2 TextMeasurement(ReadOnlySpan<char> x) {
            sb.Clear();
            return Font.MeasureString(sb.Append(x));
        }

        _wrappedText = TextUtil.WrapText(_text, TextMeasurement, Bounds.Width);
    }
}