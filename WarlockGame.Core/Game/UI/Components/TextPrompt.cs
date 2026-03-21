using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

sealed class TextPrompt: InterfaceComponent, ITextInputConsumer {
    public string Prompt { get; set; }

    public string Text => _textInput.Text;

    public int TextConsumerPriority { get; } = 1;
    private Action<string> AcceptedCallback { get; }
    private Action<string>? CancelledCallback { get; }
    private readonly TextInput _textInput;
    
    public TextPrompt(string prompt, Action<string> acceptedCallback, Action<string>? cancelledCallback) {
        Layout = Layout.WithBoundingBox(0, 220, width: 300, height: 35, Layout.Alignment.Center);

        Layer = 10;
        
        Clickable = ClickableState.Clickable;
        
        _textInput = new TextInput { Clickable = ClickableState.Ignore };

        AddComponent(_textInput);
        
        Prompt = prompt;
        AcceptedCallback = acceptedCallback;
        CancelledCallback = cancelledCallback;
    }

    public void OnTextInput(TextInputEventArgs textEvent) {
        switch (textEvent.Key) {
            case Keys.Enter:
                Close(true);
                break;
            case Keys.Escape:
                Close(false);
                break;
            default:
                _textInput.OnTextInput(textEvent);
                break;
        }
    }

    public override void OnLeftClick(Vector2 location) {
        if (_textInput.BoundingBox.Contains(location)) {
            _textInput.OnLeftClick(location - _textInput.BoundingBox.Location.ToVector2());
        }
    }

    public override void OnLostFocus() {
        Close(false);
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        var absoluteBounds = BoundingBox.WithOffset(location);
        spriteBatch.Draw(Art.Pixel, absoluteBounds, Color.DarkSlateGray);
        UiUitils.DrawHollowRectangle(spriteBatch, Art.Pixel, absoluteBounds, Color.Black, width: 2);

        spriteBatch.DrawString(Art.Font, Prompt, absoluteBounds.Location.ToVector2().Translate(0, -24), Color.White);
    }

    public void Close(bool accepted) {
        if (accepted) {
            AcceptedCallback.Invoke(Text);
        }
        else {
            CancelledCallback?.Invoke(Text);
        }
        IsExpired = true;
    }
}