using System;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class TextDisplay : InterfaceComponent {
    public string Text {
        get => _text;
        set {
            _text = value;
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
            _fontHeight = value.MeasureString(" ").Y;
            _isTextDirty = true;
        }
    }

    private float _fontHeight;
    private string _text = string.Empty;
    private string _wrappedText = string.Empty;
    private bool _isTextDirty;
    
    private readonly StringBuilder _sb = new();
    private int _cursorIndex;

    private float? CursorPositionX {
        get;
        set {
            field = value;
            _frameBlinkTimer = 0;
        }
    } = null;
    private int _frameBlinkTimer = 0;
    
    private const int BlinkSpeed = 60; // Frames between blinks
    
    public bool CursorEnabled { 
        get;
        set {
            field = value;
            if (value == false) {
                CursorPositionX = null;
            }
        }
    }

    public bool CursorVisible => CursorPositionX != null;

    public TextDisplay(string text = "", SpriteFont? font = null, bool cursorEnabled = false) {
        Text = text;
        Font = font ?? Art.Font;
        CursorEnabled = cursorEnabled;
        Clickable = ClickableState.Ignore;
    }
    
    public void Insert(string str) {
        if (CursorEnabled) {
            Text = Text.Insert(_cursorIndex, str);
            _cursorIndex += str.Length;
            CursorPositionX = GetCursorLocation(_cursorIndex);
        } else {
            Text += str;
        }
    }

    public void RemoveChar() {
        if (CursorEnabled) {
            if (_cursorIndex == 0) return;
            Text = Text.Remove(_cursorIndex - 1, 1);
            _cursorIndex--;
            CursorPositionX = GetCursorLocation(_cursorIndex);
        } else {
            Text = Text[..^1];
        }
    }
    
    public void MoveCursorRight() {
        if (_cursorIndex < Text.Length) {
            _cursorIndex++;
            CursorPositionX = GetCursorLocation(_cursorIndex);
        }    
    }
    
    public void MoveCursorLeft() {
        if (_cursorIndex > 0) {
            _cursorIndex--;
            CursorPositionX = GetCursorLocation(_cursorIndex);
        }
    }

    // Thread unsafe
    public void SetCursorToPosition(Vector2 location) {
        if (CursorEnabled) {
            (_cursorIndex, CursorPositionX) = FindCursorPositionX(location);
        }
    }
    
    public void ClearCursorPosition() {
        CursorPositionX = null;
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (BackgroundColor != null) {
            var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pointTexture.SetData([BackgroundColor.Value]);

            spriteBatch.Draw(pointTexture, BoundingBox.WithOffset(location), Color.White);
        }

        if(IsLayoutDirty || _isTextDirty) {
            RecalculateWrappedText();
        }
        
        spriteBatch.DrawString(Art.Font, _wrappedText, BoundingBox.Location.ToVector2() + location, 
            color: TextColor,
            scale: TextScale,
            rotation: 0,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            layerDepth: 0);

        if (CursorPositionX != null) {
            if (_frameBlinkTimer++ < BlinkSpeed) {
                var cursorStart = new Vector2(CursorPositionX.Value, 0);
                var cursorEnd = new Vector2(CursorPositionX.Value, _fontHeight);
                spriteBatch.DrawLine(location + cursorStart, location + cursorEnd, Color.Black);
            }

            if (_frameBlinkTimer++ > BlinkSpeed * 2) {
                _frameBlinkTimer = 0;
            }
        }
    }
    
    // Thread unsafe
    private void RecalculateWrappedText() {
        Vector2 TextMeasurement(ReadOnlySpan<char> x) {
            _sb.Clear();
            return Font.MeasureString(_sb.Append(x)) * TextScale;
        }

        _wrappedText = TextUtil.WrapText(_text, TextMeasurement, BoundingBox.Width, maxHeight: BoundingBox.Height, truncation: TruncationCharacters);
        if (CursorEnabled && _wrappedText.Contains('\n')) {
            Logger.Error("Line break with cursor enabled is not supported!", Logger.LogType.Interface);
        }
    }
    
    // Note: This implementation only works for single lines. Generalize it later if needed.
    // Thread unsafe
    private (int, float) FindCursorPositionX(Vector2 clickPosition) {
        if (Text.IsEmpty()) return (0, 0);
        
        _sb.Clear();
        _sb.Append(' ');

        float width = 0;
        for (var cursorPosition = 0; cursorPosition < Text.Length; cursorPosition++) {
            _sb[0] = Text[cursorPosition];
            var charWidth = Font.MeasureString(_sb).X * TextScale;
            if (width + charWidth * 0.6 > clickPosition.X) {
                return (cursorPosition, width);
            }

            width += charWidth;
        }

        return (Text.Length - 1, width);
    }

    // Thread unsafe
    private float GetCursorLocation(int cursorIndex) {
        _sb.Clear();
        _sb.Append(Text.AsSpan().Slice(0, cursorIndex));
        return Font.MeasureString(_sb).X * TextScale;
    }
}