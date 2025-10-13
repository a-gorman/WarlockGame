using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;
using ZLinq;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Warlock = WarlockGame.Core.Game.Sim.Entities.Warlock;

namespace WarlockGame.Core.Game.UI.Components;

sealed class MainView : InterfaceComponent {
    private readonly Simulation _sim;
    private readonly PerkPicker _perkPicker;
    
    private const int HpBarVerticalOffset = 30;
    private const int HpBarWidth = 80;
    private const int HpBarHeight = 3;

    private const int ScrollBoundaryWidth = 50;
    private const float SideScrollSpeed = 5f;

    public RectangleF ViewBounds { get; set; }
    
    public MainView(Simulation sim) {
        _sim = sim;
        Layer = -1;
        Clickable = ClickableState.Clickable;
        BoundingBox = new Rectangle(new Point(0, 0), WarlockGame.ScreenSize.ToPoint());
        ViewBounds = BoundingBox.ToRectangleF() with { X = 0, Y = 0 };

        _perkPicker = new PerkPicker(_sim, new Vector2(600, 400)) {Visible = false, Clickable = ClickableState.PassThrough};
        AddComponent(_perkPicker);
    }

    public override void OnRightClick(Vector2 location) {
        var worldLocation = location - ViewBounds.Position;
        
        var localPlayerId = PlayerManager.LocalPlayer?.Id;
        if (localPlayerId != null) {
            InputManager.HandlePlayerAction(new MoveAction { PlayerId = localPlayerId.Value, Location = worldLocation });
        }
        InputManager.SelectedSpellId = null;
    }

    public override void OnLeftClick(Vector2 location) {
        var worldLocation = location - ViewBounds.Position;
        
        if (InputManager.SelectedSpellId != null) {
            var localPlayerId = PlayerManager.LocalPlayer?.Id;
            if (localPlayerId == null) return;

            var warlock = _sim.EntityManager.GetWarlockByForceId(localPlayerId.Value);
            if (warlock == null) return;
            if (!_sim.SpellManager.Spells.TryGetValue(InputManager.SelectedSpellId.Value, out var spell)) return;

            Vector2? castVector = spell.Effect.Match<Vector2?>(
                _ => (worldLocation - warlock.Position).ToNormalizedOrZero(),
                _ => worldLocation,
                _ => null
            );

            CastAction.CastType castType = spell.Effect.Match(
                _ => CastAction.CastType.Directional,
                _ => CastAction.CastType.Location,
                _ => CastAction.CastType.Self
            );
            
            if (castVector is not null) {
                InputManager.HandlePlayerAction(new CastAction {
                    PlayerId = localPlayerId.Value,
                    Type = castType,
                    CastVector = castVector.Value,
                    SpellId = InputManager.SelectedSpellId.Value
                });
            }
        }

        InputManager.SelectedSpellId = null;
    }

    public override void Update(ref readonly UIManager.UpdateArgs args) {
        var localPlayerId = PlayerManager.LocalPlayerId;
        if (localPlayerId != null && _sim.GameRules.Statuses.TryGetValue(localPlayerId.Value, out var status)) {
            _perkPicker.Visible = status.ChoosingPerk;
        }

        if (args.Global.MousePosition.X < ScrollBoundaryWidth) {
            ViewBounds = ViewBounds with { X = ViewBounds.X - SideScrollSpeed };
        }
        if (args.Global.MousePosition.X > BoundingBox.Width - ScrollBoundaryWidth) {
            ViewBounds = ViewBounds with { X = ViewBounds.X + SideScrollSpeed };
        }
        if (args.Global.MousePosition.Y < ScrollBoundaryWidth) {
            ViewBounds = ViewBounds with { Y = ViewBounds.Y - SideScrollSpeed };
        }
        if (args.Global.MousePosition.Y > BoundingBox.Height - ScrollBoundaryWidth) {
            ViewBounds = ViewBounds with { Y = ViewBounds.Y + SideScrollSpeed };
        }
    }

    public override void Draw(Vector2 location, SpriteBatch spriteBatch) {
        DrawEntities(location, spriteBatch);
    }

    private void DrawEntities(Vector2 location, SpriteBatch spriteBatch) {
        foreach (var entity in _sim.EntityManager.EntitiesLivingOrDead) {
            if (entity.IsDead) {
                continue;
            }

            if (entity is Warlock warlock) {
                DrawWarlock(location + ViewBounds.Position, spriteBatch, entity, warlock);
            }
            else {
                entity.Sprite.Draw(spriteBatch, entity.Position + ViewBounds.Position, entity.Orientation);
            }
        }
    }

    private void DrawWarlock(Vector2 location, SpriteBatch spriteBatch, Entity entity, Warlock warlock) {
        float opacity = 1;
        if (entity.PlayerId != PlayerManager.LocalPlayerId) {
            var invisBuffs = warlock.Buffs.AsValueEnumerable().OfType<Invisibility>();
            if (invisBuffs.Any()) {
                var localPlayerPos = _sim.EntityManager.GetWarlockByForceId(PlayerManager.LocalPlayerId!.Value)
                    ?.Position;
                if (localPlayerPos != null) {
                    opacity = invisBuffs.Select(x =>
                            x.CalculateVisibility((localPlayerPos.Value - entity.Position).Length()))
                        .Min();
                }
            }
        }
        
        entity.Sprite.Draw(spriteBatch, entity.Position + location, entity.Orientation, opacity: opacity);
        DrawHealthBar(warlock, opacity, location, spriteBatch);
    }

    private void DrawHealthBar(Warlock warlock, float opacity, Vector2 location, SpriteBatch spriteBatch) {
        float filledProportion = warlock.Health / warlock.MaxHealth;

        var filledTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        filledTexture.SetData([Color.Lerp(Color.Red * opacity, Color.Green * opacity, filledProportion)]);

        var unfilledTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        unfilledTexture.SetData([Color.Black * opacity]);

        var position = warlock.Position + location;
        position.Y -= HpBarVerticalOffset;

        spriteBatch.Draw(unfilledTexture,
            new Rectangle((int)position.X - HpBarWidth / 2, (int)position.Y, HpBarWidth, HpBarHeight), 
            Color.White);
        
        spriteBatch.Draw(filledTexture,
            new Rectangle((int)position.X - HpBarWidth / 2, (int)position.Y, (int)(HpBarWidth * filledProportion), HpBarHeight),
            Color.White);
    }
}