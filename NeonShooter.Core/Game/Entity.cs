//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter.Core.Game
{
	internal abstract class Entity
	{
		// protected Texture2D? Image;
		// The tint of the image. This will also allow us to change the transparency.
		// protected Color Color = Color.White;	

		protected Sprite _sprite;
		
		public Vector2 Position, Velocity;
		public float Orientation;
		public float Radius = 20;	// used for circular collision detection
		public bool IsExpired;		// true if the entity was destroyed and should be deleted.

		// public Vector2 Size => Image == null ? Vector2.Zero : new Vector2(Image.Width, Image.Height);

		public abstract void Update();

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			_sprite.Draw(spriteBatch, Position, Orientation);
		}
	}
}