using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Log;

namespace WarlockGame.Core.Game.UI;

// This probably should not be an effect. Probably make some UI components thing
class TextPrompt: ITextInputComponent {
    public string Prompt { get; set; }

    public string Text { get; set; } = string.Empty;

    public int Layer { get; } = 1;
    public Rectangle BoundingBox { get; }
    
    public bool IsExpired { get; set; }
    
    private Vector2 Position { get; }
    private Action<string, bool> OnCloseCallback { get; }
    public event EventHandler? OnClose;
    
    public TextPrompt(string prompt, Action<string, bool> onCloseCallback) {
        Position = new Vector2(800, 800);
        BoundingBox = new Rectangle(Position.ToPoint(), new Point(300, 35));
        
        Prompt = prompt;
        OnCloseCallback = onCloseCallback;
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
    }
    
    public void OnClick(Vector2 location) {
        Logger.Info("Click on text prompt");
        // TODO: Move a cursor to the click location
    }

    public void Close(bool accepted) {
        OnCloseCallback.Invoke(Text, accepted);
        OnClose?.Invoke(this, EventArgs.Empty);
    }
}