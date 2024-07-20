using System;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Input;

internal interface ITextInputConsumer {
    public void OnTextInput(TextInputEventArgs args);
    public bool IsExpired { get; }
    /// <summary>
    /// Higher priority consumers (those with a higher number) are given the text input instead of lower priority consumers
    /// </summary>
    public int TextConsumerPriority { get; }
}