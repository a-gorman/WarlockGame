using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Entity;
using NeonShooter.Core.Game.Entity.Projectile;
using NeonShooter.Core.Game.Graphics.Effect;

namespace NeonShooter.Core.Game.Spell; 

public class LightningEffect: ICastEffect {

    private Texture2D _art = Art.Lightning;
    
    public void OnCast(Vector2 castPosition, Vector2 castDirection) {
        
        foreach (var entity in EntityManager.GetNearbyEntities(castPosition, 100))
        {
            switch (entity)
            {
                case Enemy enemy:
                    enemy.WasShot();
                    break;
                // case PlayerShip player:
                //     player.Push(20, player.Position - castPosition);
                //     break;
            }
        }
        
        EffectManager.Add(new Lightning(_art, castPosition, castDirection.ToAngle()));
    }
}