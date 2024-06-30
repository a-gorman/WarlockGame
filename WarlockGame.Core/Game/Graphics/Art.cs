using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game
{
	internal static class Art
	{
		public static Texture2D Player { get; private set; } = null!;
		public static Texture2D Seeker { get; private set; } = null!;
		public static Texture2D Wanderer { get; private set; } = null!;
		public static Texture2D Bullet { get; private set; } = null!;

		public static Texture2D Lightning { get; private set; } = null!;
		public static Texture2D LightningIcon { get; private set; } = null!;
		public static Texture2D Fireball { get; private set; } = null!;
		public static Texture2D FireballIcon { get; private set; } = null!;
		public static Texture2D Pointer { get; private set; } = null!;
		public static Texture2D BlackHole { get; private set; } = null!;

		public static Texture2D LineParticle { get; private set; } = null!;
		public static Texture2D Glow { get; private set; } = null!;
		public static Texture2D Pixel { get; private set; } = null!;		// a single white pixel

		public static SpriteFont Font { get; private set; } = null!;

		public static void Load(ContentManager content)
		{
			Player = content.Load<Texture2D>("Art/Player");
			Seeker = content.Load<Texture2D>("Art/Seeker");
			Wanderer = content.Load<Texture2D>("Art/Wanderer");
			Bullet = content.Load<Texture2D>("Art/Bullet");
			Lightning = content.Load<Texture2D>("Art/Lightning");
			LightningIcon = content.Load<Texture2D>("Art/Air_07_Call_Lightning");
			Fireball = content.Load<Texture2D>("Art/Fireball");
			FireballIcon = content.Load<Texture2D>("Art/Fire_03_Meteor");
			Pointer = content.Load<Texture2D>("Art/Pointer");
			BlackHole = content.Load<Texture2D>("Art/Black Hole");

			LineParticle = content.Load<Texture2D>("Art/Laser");
			Glow = content.Load<Texture2D>("Art/Glow");

			Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
			Pixel.SetData(new[] { Color.White });

			Font = content.Load<SpriteFont>("Font");
		}
		
		public static Texture2D LoadTexture(string assetName)
		{
			var texture = Texture2D.FromFile(WarlockGame.Instance.GraphicsDevice, assetName);
			return texture;
		}
	}
}