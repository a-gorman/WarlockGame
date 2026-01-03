using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components.Basic;

class ToggleSelection : InterfaceComponent {
    public Action<Vector2>? LeftClick { get; set; }
    public Action<Vector2>? RightClick { get; set; }
    
    public bool IsSelected { get; set; }
    
    private readonly Texture2D _texture;
    private readonly Color _clickedBorderColor;
    private const int BorderThickness = 4;
    
    public ToggleSelection(Rectangle boundingBox, Texture2D texture, Color clickedBorderColor) {
        _clickedBorderColor = clickedBorderColor;
        Clickable = ClickableState.Clickable;
        _texture = texture;
        BoundingBox = boundingBox;
    }
    
    public override void OnLeftClick(Vector2 location) {
        IsSelected = !IsSelected;
        LeftClick?.Invoke(location);
    }

    public override void OnRightClick(Vector2 location) {
        RightClick?.Invoke(location);
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        spriteBatch.Draw(_texture, BoundingBox.WithOffset(location), Color.White);
        if (IsSelected) {
            spriteBatch.DrawRectangle(BoundingBox.WithOffset(location).ToRectangleF(), _clickedBorderColor, BorderThickness);
        }
    }
}