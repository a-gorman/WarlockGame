using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;

namespace WarlockGame.Core.Game.Sim.Entity
{
	abstract class EntityBase : IEntity
	{
		protected readonly Sprite _sprite;
		protected readonly Simulation _simulation;

		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Orientation { get; set; }
		public float Radius { get; init; } = 20;	// used for circular collision detection
		public bool IsExpired { get; set; }		// true if the entity was destroyed and should be deleted.

		protected EntityBase(Sprite sprite, Simulation simulation) {
			_sprite = sprite;
			_simulation = simulation;
		}
		
		public abstract void Update();

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			_sprite.Draw(spriteBatch, Position, Orientation);
		}
	}
}