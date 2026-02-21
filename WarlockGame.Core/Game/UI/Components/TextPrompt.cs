using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using TextCopy;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Input.Devices;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.UI.Components.Basic;

namespace WarlockGame.Core.Game.UI.Components;

sealed class TextPrompt: InterfaceComponent, ITextInputConsumer {
    public string Prompt { get; set; }

    public string Text { get => _textDisplay.Text; set => _textDisplay.Text = value; }

    public int TextConsumerPriority { get; } = 1;
    public int MaxCharacters { get; } = 30;
    private Action<string> AcceptedCallback { get; }
    private Action<string>? CancelledCallback { get; }
    private readonly TextDisplay _textDisplay;
    
    public TextPrompt(string prompt, Action<string> acceptedCallback, Action<string>? cancelledCallback) {
        BoundingBox = new Rectangle(new Point(800, 800), new Point(300, 35));

        _textDisplay = new TextDisplay
        {
            Layer = 1
        };

        AddComponent(_textDisplay);
        
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
                if (Text.Length < MaxCharacters && IsStandardCharacter(textEvent.Character)) {
                    Text += textEvent.Character;
                }
                break;
        }
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pointTexture.SetData([Color.DarkSlateGray]);

        BoundingBox.Offset(location);
        spriteBatch.Draw(pointTexture, BoundingBox, Color.White);

        spriteBatch.DrawString(Art.Font, Prompt, BoundingBox.Location.ToVector2().Translate(0, -24) + location, Color.White);
    }
    
    public override void OnLeftClick(Vector2 location) {
        Logger.Debug("Click on text prompt", Logger.LogType.Interface | Logger.LogType.PlayerAction);
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

    private bool IsStandardCharacter(char character) {
        // Space to ~ in UTF-16
        return character >= 0x20 && character <= 0x7e;
    }
}