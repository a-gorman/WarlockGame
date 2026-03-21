using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Input;

internal interface ITextInputConsumer {
    public void OnTextInput(TextInputEventArgs args);
    public bool IsExpired { get; }
    public int TextConsumerPriority { get; }
}