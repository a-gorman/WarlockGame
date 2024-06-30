using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WarlockGame.Core.Game.Graphics.Effect;

namespace WarlockGame.Core.Game.Input;

// This probably should not be an effect. Probably make some UI components thing
class TextPrompt: IEffect {
    public string Prompt { get; set; }

    public string Text { get; set; } = string.Empty;

    public Vector2 Position { get; } = new Vector2(800, 800);
    
    public bool IsExpired { get; set; }
    
    private Action<string, bool> OnClose { get; }
    
    private TextPrompt(string prompt, Action<string, bool> onClose) {
        Prompt = prompt;
        OnClose = onClose;
        WarlockGame.Instance.Window.TextInput += AddText;
    }

    // TODO: This needs to block keyboard input from the game. Should be handled elsewhere
    public static TextPrompt Open(string prompt, Action<string, bool> onClose) {
        var textPrompt = new TextPrompt(prompt, onClose);
        EffectManager.Add(textPrompt);
        return textPrompt;
    }

    private void AddText(object? source, TextInputEventArgs textEvent) {
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

        spriteBatch.DrawString(Art.Font, Text, Position, Color.White);
        spriteBatch.Draw(pointTexture, new Rectangle(Position.ToPoint(), new Point(300,35)), Color.White);
    }

    public void Close(bool accepted) {
        WarlockGame.Instance.Window.TextInput -= AddText;
        OnClose.Invoke(Text, accepted);
        IsExpired = true;
    }
}