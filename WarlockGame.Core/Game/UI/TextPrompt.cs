using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using TextCopy;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.UI.Basic;

namespace WarlockGame.Core.Game.UI;

class TextPrompt: InterfaceComponent, ITextInputConsumer {
    public string Prompt { get; set; }

    public string Text { get => _textDisplay.Text; set => _textDisplay.Text = value; }

    public override Rectangle BoundingBox { get => _textDisplay.Bounds; set => _textDisplay.Bounds = value; }
    public int TextConsumerPriority { get; } = 1;
    public int MaxCharacters { get; } = 30;
    private Vector2 Position { get; }
    private Action<string> AcceptedCallback { get; }
    private Action<string>? CancelledCallback { get; }
    private readonly TextDisplay _textDisplay;
    
    public TextPrompt(string prompt, Action<string> acceptedCallback, Action<string>? cancelledCallback) {
        Position = new Vector2(800, 800);
        var boundingBox = new Rectangle(Position.ToPoint(), new Point(300, 35));

        _textDisplay = new TextDisplay
        {
            Bounds = boundingBox,
            Layer = 1
        };

        _components.Add(_textDisplay);
        
        Prompt = prompt;
        AcceptedCallback = acceptedCallback;
        CancelledCallback = cancelledCallback;
    }

    public void OnTextInput(TextInputEventArgs textEvent) {
        if(StaticKeyboardInput.IsKeyPressed(Keys.LeftControl) || StaticKeyboardInput.IsKeyPressed(Keys.RightControl)) {
            switch (textEvent.Key) {
                case Keys.V:
                    // This gets around max text size limits
                    Text += ClipboardService.GetText();
                    break;
            }

            return;
        }
        
        switch (textEvent.Key) {
            case Keys.Enter:
                Close(true);
                break;
            case Keys.Escape:
                Close(false);
                break;
            case Keys.Back:
                if (Text.Length > 0) {
                    Text = Text.Remove(Text.Length - 1);
                }
                break;
            default:
                if (Text.Length < MaxCharacters) {
                    Text += textEvent.Character;
                }
                break;
        }
    }

    public override void Draw(SpriteBatch spriteBatch) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pointTexture.SetData(new[] { Color.DarkSlateGray });
        spriteBatch.Draw(pointTexture, BoundingBox, Color.White);

        spriteBatch.DrawString(Art.Font, Prompt, Position.Translate(0, -24), Color.White);
    }
    
    public override void OnClick(Vector2 location) {
        Logger.Debug("Click on text prompt");
        // TODO: Move a cursor to the click location
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