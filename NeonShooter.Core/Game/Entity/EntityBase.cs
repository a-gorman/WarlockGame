//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Display;
using NeonShooter.Core.Game.Graphics;

namespace NeonShooter.Core.Game.Entity
{
	abstract class EntityBase : IEntity
	{
		protected readonly Sprite _sprite;
		
		public int Id { get; init; }
		
		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Orientation { get; set; }
		public float Radius { get; init; } = 20;	// used for circular collision detection
		public bool IsExpired { get; set; }		// true if the entity was destroyed and should be deleted.

		protected EntityBase(Sprite sprite)
		{
			_sprite = sprite;
		}
		
		public abstract void Update();

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			_sprite.Draw(spriteBatch, Position, Orientation);
		}
	}
}