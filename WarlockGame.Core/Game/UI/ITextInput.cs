using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.UI;

internal interface ITextInputComponent: IUIComponent {
    public void OnTextInput(TextInputEventArgs args);
}