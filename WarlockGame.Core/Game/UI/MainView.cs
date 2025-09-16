using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.UI.Components;
using WarlockGame.Core.Game.Util;
using ZLinq;
using Warlock = WarlockGame.Core.Game.Sim.Entities.Warlock;

namespace WarlockGame.Core.Game.UI;

sealed class MainView : InterfaceComponent {
    private readonly Simulation _sim;
    
    private const int HpBarVerticalOffset = 30;
    private const int HpBarWidth = 80;
    private const int HpBarHeight = 3;

    public MainView(Simulation sim) {
        _sim = sim;
        Layer = -1;
        Clickable = ClickableState.Consume;
        BoundingBox = new Rectangle(new Point(0, 0), WarlockGame.ScreenSize.ToPoint());
    }

    public override void OnRightClick(Vector2 location) {
        var localPlayerId = PlayerManager.LocalPlayer?.Id;
        if (localPlayerId != null) {
            InputManager.IssueCommand(new MoveCommand { PlayerId = localPlayerId.Value, Location = location });
        }
        InputManager.SelectedSpellId = null;
    }

    public override void OnLeftClick(Vector2 location) {
        if (InputManager.SelectedSpellId != null) {
            var localPlayerId = PlayerManager.LocalPlayer?.Id;
            if (localPlayerId == null) return;

            var warlock = _sim.EntityManager.GetWarlockByForceId(localPlayerId.Value);
            if (warlock == null) return;
            if (!_sim.SpellManager.Spells.TryGetValue(InputManager.SelectedSpellId.Value, out var spell)) return;

            Vector2? castVector = spell.Effect.Match<Vector2?>(
                _ => (location - warlock.Position).ToNormalizedOrZero(),
                _ => location,
                _ => null
            );

            CastCommand.CastType castType = spell.Effect.Match(
                _ => CastCommand.CastType.Directional,
                _ => CastCommand.CastType.Location,
                _ => CastCommand.CastType.Self
            );
            
            if (castVector is not null) {
                InputManager.IssueCommand(new CastCommand {
                    PlayerId = localPlayerId.Value,
                    Type = castType,
                    CastVector = castVector.Value,
                    SpellId = InputManager.SelectedSpellId.Value
                });
            }
        }

        InputManager.SelectedSpellId = null;
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
                DrawWarlock(location, spriteBatch, entity, warlock);
            }
            else {
                entity.Sprite.Draw(spriteBatch, entity.Position, entity.Orientation);
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
        
        entity.Sprite.Draw(spriteBatch, entity.Position, entity.Orientation, opacity: opacity);
        DrawHealthBar(warlock, opacity, location, spriteBatch);
    }

    private void DrawHealthBar(Warlock warlock, float opacity, Vector2 location, SpriteBatch spriteBatch) {
        float filledProportion = warlock.Health / warlock.MaxHealth;

        var filledTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        filledTexture.SetData([Color.Lerp(Color.Red * opacity, Color.Green * opacity, filledProportion)]);

        var unfilledTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        unfilledTexture.SetData([Color.Black * opacity]);

        var position = warlock.Position;
        position.Y -= HpBarVerticalOffset;

        spriteBatch.Draw(unfilledTexture,
            new Rectangle((int)position.X - HpBarWidth / 2, (int)position.Y, HpBarWidth, HpBarHeight), 
            Color.White);
        
        spriteBatch.Draw(filledTexture,
            new Rectangle((int)position.X - HpBarWidth / 2, (int)position.Y, (int)(HpBarWidth * filledProportion), HpBarHeight),
            Color.White);
    }
}