using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Perks;
using WarlockGame.Core.Game.UI.Components.Basic;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI.Components;

sealed class PerkPicker: InterfaceComponent {
    private const int PerkSelections = 3;
    
    private readonly Simulation _sim;
    private List<Perk> _perks = new();
    private readonly int _marginX = 20;
    private readonly int _marginY = 20;

    private readonly TextDisplay _pickingTimeDisplay;
    
    private TimeSpan _pickingEndTime;

    private bool _hasPicked;
    
    private Texture2D? _rainbowTexture;
    
    public PerkPicker(Simulation sim) {
        _sim = sim;
        Layout = Layout.WithSize(600, 200, Layout.Alignment.Center);
        _pickingTimeDisplay = new TextDisplay {
            Layout = Layout.WithHeight((int)(Art.FontHeight * 0.75f), alignment: Layout.Alignment.Bottom),
            TextScale = 0.75f,
            TextColor = Color.Black
        };
    }

    public override void Update(ref readonly UIManager.UpdateArgs args) {
        if (WasMadeVisible) {
            var perks = _sim.PerkManager.GetAvailablePerks(PlayerManager.LocalPlayerId ?? -1);
            if (perks.Length > 0) {
                _hasPicked = false;
                _pickingEndTime = WarlockGame.GameTime.TotalGameTime + TimeSpan.FromSeconds(10);
                perks.Shuffle(Random.Shared);
                SetPerks(perks.Take(PerkSelections));
            } else {
                Logger.Error("Could not get perks for local player!", Logger.LogType.Interface | Logger.LogType.Simulation);
            }
        }

        if (Visible && !_hasPicked) {
            var timeRemaining = _pickingEndTime - WarlockGame.GameTime.TotalGameTime;
            _pickingTimeDisplay.Text = $"Time remaining: {Math.Max(timeRemaining.Seconds, 0)} seconds";

            if (timeRemaining.Ticks <= 0) {
                var playerId = PlayerManager.LocalPlayerId;
                if (playerId != null) {
                    _hasPicked = true;
                    var randomPerk = _perks[Random.Shared.Next(0, _perks.Count)];
                    Logger.Info($"Perk picking time expired, randomly picked perk: {randomPerk.Name}", Logger.LogType.Interface);
                    InputManager.HandlePlayerAction(new SelectPerk { PlayerId = playerId.Value, PerkId = randomPerk.Id });
                }
            }
        }
    }

    private void SetPerks(IEnumerable<Perk> perks) {
        RemoveAllComponents();
        AddComponent(_pickingTimeDisplay);
        _perks = perks.ToList();
        var grid = new Grid(BoundingBox.AtOrigin().WithMargin(_marginX, _marginY), _perks.Count, 1) {
            Clickable = ClickableState.PassThrough
        };
        AddComponent(grid);
        for (var index = 0; index < _perks.Count; index++) {
            var perk = _perks[index];
            var button = new Button(perk.Texture) {
                Layout = Layout.WithMargin(15),
                LeftClick = _ => {
                    var playerId = PlayerManager.LocalPlayerId;
                    if (playerId == null || _hasPicked) return;
                    _hasPicked = true;
                    InputManager.HandlePlayerAction(new SelectPerk { PlayerId = playerId.Value, PerkId = perk.Id });
                }
            };

            grid.AddComponentToCell(button, column: index, row: 0);
            button.AddComponent(new TextDisplay(perk.Name) { TextScale = 0.5f});
        }
    }

    protected override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        if (_rainbowTexture == null) {
            _rainbowTexture = new Texture2D(spriteBatch.GraphicsDevice, 2, 2);
            _rainbowTexture.SetData([Color.Red, Color.Blue, Color.White, Color.Green]);
        }
        
        spriteBatch.Draw(_rainbowTexture, BoundingBox.WithOffset(location), Color.White);
        UiUitils.DrawHollowRectangle(spriteBatch, Art.Pixel, BoundingBox.WithOffset(location), Color.Black, 2);
    }
}