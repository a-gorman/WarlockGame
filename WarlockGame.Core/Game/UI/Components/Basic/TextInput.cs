using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TextCopy;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Input.Devices;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class TextInput : InterfaceComponent, ITextInputConsumer {
    public string Text => _textDisplay.Text;

    public int TextConsumerPriority => 0;
    
    public int MaxCharacters { get; } = 30;
    
    private readonly TextDisplay _textDisplay;
    
    public TextInput(SpriteFont? font = null, Color? textColor = null, Color? backgroundColor = null) {
        Clickable = ClickableState.Clickable;
        _textDisplay = new TextDisplay(font: font, cursorEnabled: true) {
            TextColor = textColor ?? Color.White,
            BackgroundColor = backgroundColor
        };
        AddComponent(_textDisplay);
    }
    
    public void OnTextInput(TextInputEventArgs textEvent) {
        if (StaticKeyboardInput.IsKeyPressed(Keys.LeftControl) ||
            StaticKeyboardInput.IsKeyPressed(Keys.RightControl)) {
            switch (textEvent.Key) {
                case Keys.V:
                    var pasteText = ClipboardService.GetText();
                    if (pasteText != null && pasteText.Length + Text.Length <= MaxCharacters) {
                        _textDisplay.Insert(pasteText);
                    }
                    break;
            }
            return;
        }

        switch (textEvent.Key) {
            case Keys.Back:
                if (Text.Length > 0) {
                    _textDisplay.RemoveChar();
                }
                break;
            default:
                if (Text.Length < MaxCharacters && IsStandardCharacter(textEvent.Character)) {
                    _textDisplay.Insert(textEvent.Character.ToString());
                }
                break;
        }
    }
    
    public override void Update(ref readonly UIManager.UpdateArgs args) {
        if (_textDisplay.CursorVisible) {
            if (args.Global.InputState.WasActionKeyPressed(InputAction.MoveLeft)) {
                _textDisplay.MoveCursorLeft();
            }
            if (args.Global.InputState.WasActionKeyPressed(InputAction.MoveRight)) {
                _textDisplay.MoveCursorRight();
            }
        }
    }

    public override void OnLeftClick(Vector2 location) {
        _textDisplay.SetCursorToPosition(location);
        UIManager.RegisterTextConsumer(this);
    }
    
    public override void OnLostFocus() {
        _textDisplay.ClearCursorPosition();
        UIManager.RemoveTextConsumer(this);
    }

    private bool IsStandardCharacter(char character) {
        // Space to ~ in UTF-16
        return character >= 0x20 && character <= 0x7e;
    }
}