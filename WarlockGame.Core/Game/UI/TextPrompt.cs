using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

// This probably should not be an effect. Probably make some UI components thing
class TextPrompt: ITextInputConsumer, IUIComponent {
    public string Prompt { get; set; }

    public string Text { get; set; } = string.Empty;

    public int Layer { get; } = 1;
    public Rectangle BoundingBox { get; }
    public int TextConsumerPriority { get; } = 1;
    public bool IsExpired { get; private set; }

    private Vector2 Position { get; }
    private Action<string> AcceptedCallback { get; }
    private Action<string>? CancelledCallback { get; }
    
    public TextPrompt(string prompt, Action<string> acceptedCallback, Action<string>? cancelledCallback) {
        Position = new Vector2(800, 800);
        BoundingBox = new Rectangle(Position.ToPoint(), new Point(300, 35));
        
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
            case Keys.Back:
                if (Text.Length > 0) {
                    Text = Text.Remove(Text.Length - 1);
                }
                break;
            default:
                Text += textEvent.Character;
                break;
        }
    }

    public void Update() {}
    
    public void Draw(SpriteBatch spriteBatch) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pointTexture.SetData(new[] { Color.DarkSlateGray });
        spriteBatch.Draw(pointTexture, BoundingBox, Color.White);

        spriteBatch.DrawString(Art.Font, Text, Position, Color.White);
        spriteBatch.DrawString(Art.Font, Prompt, Position.Translate(0, -24), Color.White);
    }
    
    public void OnClick(Vector2 location) {
        Logger.Info("Click on text prompt");
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