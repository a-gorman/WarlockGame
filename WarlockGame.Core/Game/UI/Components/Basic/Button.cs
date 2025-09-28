using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

sealed class Button: InterfaceComponent {
    public Texture2D Texture { get; set; }
    public Action<Vector2> LeftClick = _ => { };
    public Action<Vector2> RightClick = _ => { };
    
    public Button(Rectangle boundingBox, Texture2D texture) {
        Clickable = ClickableState.Consume;
        Texture = texture;
        BoundingBox = boundingBox;
    }
    
    public override void OnLeftClick(Vector2 location) {
        LeftClick.Invoke(location);
    }

    public override void OnRightClick(Vector2 location) {
        RightClick.Invoke(location);
    }

    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.Draw(Texture, BoundingBox.AtOffset(location), Color.White);
    }
}