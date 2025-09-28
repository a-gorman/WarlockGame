using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Perks;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

sealed class PerkPicker: InterfaceComponent {
    private readonly Simulation _sim;
    private List<Perk> _perks = new();
    private readonly int _width = 600;
    private readonly int _height = 200;
    private readonly int _marginX = 20;
    private readonly int _marginY = 20;
    
    public PerkPicker(Simulation sim, Vector2 location) {
        _sim = sim;
        BoundingBox = new Rectangle(location.ToPoint(), new Point(_width, _height));
    }

    public override void Update() {
        if (IsDirty) {
            SetPerks(_sim.PerkManager.GetAvailablePerks(PlayerManager.LocalPlayerId ?? -1));
            IsDirty = false;
        }
    }

    public void SetPerks(List<Perk> perks) {
        RemoveAllComponents();
        _perks = perks.ToList();
        var grid = new Basic.Grid(BoundingBox.AtOrigin().WithMargin(_marginX, _marginY), _perks.Count, 1) {
            Clickable = ClickableState.PassThrough
        };
        AddComponent(grid);
        for (var index = 0; index < _perks.Count; index++) {
            var perk = _perks[index];
            var button = new Button(new Rectangle(0,0, 90, 90), Art.HealIcon) {
                LeftClick = _ => {
                    var playerId = PlayerManager.LocalPlayerId;
                    if (playerId == null) return;

                    InputManager.HandlePlayerAction(new SelectPerk { PlayerId = playerId.Value, PerkType = perk.Type });
                }
            };

            grid.AddComponent(button, column: index, row: 0);
        }
    }

    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        DrawHollowRectangle(spriteBatch, BoundingBox.AtOffset(location), Color.White);
    }
    
    private static void DrawHollowRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int width = 1) {
        var pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 2, 2);
        pointTexture.SetData([Color.Red, Color.Blue, Color.White, Color.Green]);
        
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, width), color); // Bottom line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Top, width, rectangle.Height), color);   // Left line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Right, rectangle.Top, width, rectangle.Height), color);  // Right line
        spriteBatch.Draw(pointTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, width), color);    // Top line
    }
}