using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class TextDisplay : InterfaceComponent {
    public string Text {
        get => _text;
        set {
            _text = value;
            _textDirty = true;
        }
    }

    public Color TextColor { get; set; } = Color.White;
    public float TextScale { get; set; } = 1f;

    public string TruncationCharacters {
        get;
        set {
            field = value;
            _textDirty = true;
        }
    } = "";

    public SpriteFont Font {
        get;
        set {
            field = value;
            _fontHeight = value.MeasureString(" ").Y;
            _textDirty = true;
        }
    }

    private float _fontHeight;
    private string _text = string.Empty;
    private string _wrappedText = string.Empty;
    private bool _textDirty = false;

    public TextDisplay(SpriteFont? font = null) {
        Font = font ?? Art.Font;
        Clickable = ClickableState.Ignore;
    }
    
    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if(IsBoundsDirty || _textDirty) {
            RecalculateWrappedText();
        }

        spriteBatch.DrawString(Art.Font, _wrappedText, BoundingBox.Location.ToVector2() + location, TextColor,
            scale: TextScale,
            rotation: 0,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            layerDepth: 0);
    }

    private void RecalculateWrappedText() {
        var sb = new StringBuilder();
        Vector2 TextMeasurement(ReadOnlySpan<char> x) {
            sb.Clear();
            return Font.MeasureString(sb.Append(x)) * TextScale;
        }

        _wrappedText = TextUtil.WrapText(_text, TextMeasurement, BoundingBox.Width, maxHeight: BoundingBox.Height, truncation: TruncationCharacters);
    }
}