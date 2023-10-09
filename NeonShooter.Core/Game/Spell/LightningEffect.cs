using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.Graphics.Effect;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Spell; 

public class LightningEffect: ICastEffect {

    private Texture2D _art = Art.Lightning;

    private const int Lenght = 1000;
    
    public void OnCast(IEntity caster, Vector2 castDirection) {
        
        var startPoint = caster.Position + caster.Radius * castDirection.ToNormalized();
        var endPoint = startPoint + castDirection * Lenght;
            
        // Debug.Visualize(castDirection * Lenght, startPoint, Color.Red, 100);
        
        foreach (var entity in EntityManager.GetNearbyEntities(caster.Position, 100)) {
            switch (entity)
            {
                case Enemy enemy:
                    enemy.WasShot();
                    break;
                case PlayerShip player:
                    if (ReferenceEquals(player, caster)) { break; }
                    
                    player.Push(20, player.Position - caster.Position);
                    break;
            }
        }
        
        EffectManager.Add(new Lightning(_art, startPoint, castDirection.ToAngle()));
    }
}