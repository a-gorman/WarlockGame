using System;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Networking;

internal interface ITextInputConsumer {
    public void OnTextInput(TextInputEventArgs args);
    public event EventHandler OnClose;
    public int TextConsumerPriority { get; }
}