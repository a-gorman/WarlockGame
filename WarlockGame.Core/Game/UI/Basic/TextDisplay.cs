using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Basic;

class TextDisplay : IInterfaceComponent {
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

    public int Layer { get; set; }
    public bool IsExpired { get; set; }
    public bool Visible { get; set; } = true;
    public IEnumerable<IInterfaceComponent> Components { get; } = new List<IInterfaceComponent>();

    public SpriteFont Font {
        get => field;
        set {
            field = value;
            _fontHeight = value.MeasureString(" ").Y;
        }
    }

    private float _fontHeight;

    public Color TextColor { get; set; } = Color.White;

    public TextDisplay(SpriteFont? font = null) {
        Font = font ?? Art.Font;
    }
    
    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.DrawString(Art.Font, _wrappedText, Bounds.Location.ToVector2(), TextColor);
    }

    private void RecalculateWrappedText() {
        Vector2 TextMeasurement(ReadOnlySpan<char> x) => Font.MeasureString(x.ToString());
        _wrappedText = TextUtil.WrapText(_text, TextMeasurement, Bounds.Width);
    }
}