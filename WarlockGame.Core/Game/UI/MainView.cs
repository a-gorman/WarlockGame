using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Input;
using WarlockGame.Core.Game.Networking.Packet;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Sim.Perks;
using WarlockGame.Core.Game.Util;
using Warlock = WarlockGame.Core.Game.Sim.Entities.Warlock;

namespace WarlockGame.Core.Game.UI;

sealed class MainView : InterfaceComponent {
    private readonly Simulation _sim;

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

            CastCommand.CastType castType = spell.Effect.Match<CastCommand.CastType>(
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

            if (entity.PlayerId != PlayerManager.LocalPlayerId && entity is Warlock) {
                var perk = _sim.PerkManager.GetPlayerPerk(entity.PlayerId!.Value, Perk.PerkType.Invisibility);
                if (perk is InvisibilityPerk invisibilityPerk) {
                    var localPlayerPos = _sim.EntityManager.GetWarlockByForceId(PlayerManager.LocalPlayerId!.Value)
                        ?.Position;
                    if (localPlayerPos != null) {
                        entity.Sprite.Draw(
                            spriteBatch,
                            entity.Position - location,
                            entity.Orientation,
                            opacity: invisibilityPerk.CalculateVisibility((localPlayerPos.Value - entity.Position)
                                .Length()));
                        continue;
                    }
                }
            }

            entity.Sprite.Draw(spriteBatch, entity.Position, entity.Orientation);
        }
    }
}