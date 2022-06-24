//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Display;

namespace NeonShooter.Core.Game
{
	internal abstract class Entity
	{
		protected readonly Sprite _sprite;
		
		public Vector2 Position, Velocity;
		public float Orientation;
		public float Radius = 20;	// used for circular collision detection
		public bool IsExpired;		// true if the entity was destroyed and should be deleted.

		protected Entity(Sprite sprite)
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