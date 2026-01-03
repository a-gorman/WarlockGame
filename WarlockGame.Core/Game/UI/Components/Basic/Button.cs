using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

sealed class Button: InterfaceComponent {
    public Texture2D Texture { get; set; }
    public Texture2D? InactiveTexture { get; set; }
    public Action<Vector2>? LeftClick { get; set; }
    public Action<Vector2>? RightClick { get; set; }
    public bool IsActive { get; set; } = true;

    public Button(Rectangle boundingBox, Texture2D texture, Texture2D? inactiveTexture = null) {
        Clickable = ClickableState.Clickable;
        BoundingBox = boundingBox;
        Texture = texture;
        InactiveTexture = inactiveTexture;
    }

    public override void OnLeftClick(Vector2 location) {
        if (IsActive)
            LeftClick?.Invoke(location);
    }

    public override void OnRightClick(Vector2 location) {
        if (IsActive)
            RightClick?.Invoke(location);
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if(IsActive) {
            spriteBatch.Draw(Texture, BoundingBox.WithOffset(location), Color.White);
        } else if(InactiveTexture != null) {
            spriteBatch.Draw(InactiveTexture, BoundingBox.WithOffset(location), Color.White);
        }
    }
}