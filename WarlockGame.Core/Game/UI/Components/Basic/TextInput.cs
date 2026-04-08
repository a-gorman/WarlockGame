using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TextCopy;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class TextInput : InterfaceComponent, ITextInputConsumer {
    public string Text {
        get {
            if (_isTextDirty || field == null) {
                field = _textBuilder.ToString();
            }

            return field;
        }
    }

    public Color? BackgroundColor { get; set; } = null;
    public Color TextColor { get; set; } = Color.White;
    public float TextScale { get; set; } = 1f;

    public SpriteFont Font {
        get;
        set {
            field = value;
            _fontHeight = value.MeasureString(" ").Y;
            _isTextDirty = true;
        }
    }

    private float _fontHeight;
    private bool _isTextDirty;
    
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

    public int TextConsumerPriority => 0;
    
    public int MaxCharacters { get; } = 30;

    private readonly StringBuilder _textBuilder = new();
    
    private readonly StringBuilder _sb = new();
    
    public TextInput(SpriteFont? font = null, Color? textColor = null, Color? backgroundColor = null) {
        Clickable = ClickableState.Clickable;
        TextColor = textColor ?? Color.White;
        BackgroundColor = backgroundColor;
        Font = font ?? Art.Font;
        CursorEnabled = true;
    }
    
    public void OnTextInput(TextInputEventArgs textEvent) {
        if (StaticKeyboardInput.IsKeyPressed(Keys.LeftControl) ||
            StaticKeyboardInput.IsKeyPressed(Keys.RightControl)) {
            switch (textEvent.Key) {
                case Keys.V:
                    var pasteText = ClipboardService.GetText();
                    if (pasteText != null && pasteText.Length + _textBuilder.Length <= MaxCharacters) {
                        Insert(pasteText);
                    }
                    break;
            }
            return;
        }

        switch (textEvent.Key) {
            case Keys.Escape:
                OnLostFocus();
                break;
            case Keys.Back:
                if (_textBuilder.Length > 0) {
                    RemoveChar();
                }
                break;
            default:
                if (_textBuilder.Length < MaxCharacters && IsStandardCharacter(textEvent.Character)) {
                    Insert(textEvent.Character.ToString());
                }
                break;
        }
    }
    
    public override void Update(ref readonly UIManager.UpdateArgs args) {
        if (CursorVisible) {
            if (args.Global.InputState.WasActionKeyPressed(InputAction.MoveLeft)) {
                MoveCursorLeft();
            }
            if (args.Global.InputState.WasActionKeyPressed(InputAction.MoveRight)) {
                MoveCursorRight();
            }
        }
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (BackgroundColor != null) {
            spriteBatch.Draw(Art.Pixel, BoundingBox.WithOffset(location), BackgroundColor.Value);
        }

        var offset = location + BoundingBox.Location.ToVector2();
        
        spriteBatch.DrawString(Art.Font, _textBuilder, 
            position: offset, 
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
                spriteBatch.DrawLine(offset + cursorStart, offset + cursorEnd, Color.Black);
            }

            if (_frameBlinkTimer++ > BlinkSpeed * 2) {
                _frameBlinkTimer = 0;
            }
        }
    }

    public void Insert(string str) {
        if (CursorEnabled) {
            _textBuilder.Insert(_cursorIndex, str);
            _cursorIndex += str.Length;
            CursorPositionX = GetCursorLocation(_cursorIndex);
        } else {
            _textBuilder.Append(str);
        }

        _isTextDirty = true;
    }

    public void RemoveChar() {
        if (_textBuilder.Length == 0) return;
        
        if (CursorEnabled) {
            if (_cursorIndex == 0) return;
            _textBuilder.Remove(_cursorIndex - 1, 1);
            _cursorIndex--;
            CursorPositionX = GetCursorLocation(_cursorIndex);
        } else {
            _textBuilder.Remove(_textBuilder.Length - 1, 1);
        }
        
        _isTextDirty = true;
    }

    public void MoveCursorRight() {
        if (_cursorIndex < _textBuilder.Length) {
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

    public override void OnLeftClick(Vector2 location) {
        SetCursorToPosition(location);
        UIManager.RegisterTextConsumer(this);
    }
    
    public override void OnLostFocus() {
        ClearCursorPosition();
        UIManager.RemoveTextConsumer(this);
    }

    private bool IsStandardCharacter(char character) {
        // Space to ~ in UTF-16
        return character >= 0x20 && character <= 0x7e;
    }
    
    // Note: This implementation only works for single lines. Generalize it later if needed.
    // Thread unsafe
    private (int, float) FindCursorPositionX(Vector2 clickPosition) {
        if (_textBuilder.Length == 0) return (0, 0);
        
        _sb.Clear();
        _sb.Append(' ');

        float width = 0;
        for (var cursorPosition = 0; cursorPosition < _textBuilder.Length; cursorPosition++) {
            _sb[0] = _textBuilder[cursorPosition];
            var charWidth = Font.MeasureString(_sb).X * TextScale;
            if (width + charWidth * 0.6 > clickPosition.X) {
                return (cursorPosition, width);
            }

            width += charWidth;
        }

        return (_textBuilder.Length - 1, width);
    }

    // Thread unsafe
    private float GetCursorLocation(int cursorIndex) {
        _sb.Clear();
        _sb.Append(_textBuilder, 0, cursorIndex);
        return Font.MeasureString(_sb).X * TextScale;
    }
}