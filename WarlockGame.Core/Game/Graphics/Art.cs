using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WarlockGame.Core.Game.Graphics
{
	internal static class Art
	{
		public static Texture2D Player { get; private set; } = null!;
		public static Texture2D Seeker { get; private set; } = null!;
		public static Texture2D Wanderer { get; private set; } = null!;
		public static Texture2D Bullet { get; private set; } = null!;

		public static Texture2D Lightning { get; private set; } = null!;
		public static Texture2D Fireball { get; private set; } = null!;
		public static Texture2D PoisonBall { get; private set; } = null!;
		public static Texture2D Pointer { get; private set; } = null!;
		public static Texture2D BlackHole { get; private set; } = null!;

		public static Texture2D LineParticle { get; private set; } = null!;
		public static Texture2D Glow { get; private set; } = null!;
		public static Texture2D Pixel { get; private set; } = null!;		// a single white pixel

		public static SpriteFont Font { get; private set; } = null!;
		
		public static Texture2D FireballIcon { get; private set; } = null!;
		public static Texture2D LightningIcon { get; private set; } = null!;
		public static Texture2D PoisonIcon { get; private set; } = null!;
		public static Texture2D BurstIcon { get; private set; } = null!;
		public static Texture2D WindWallIcon { get; private set; } = null!;


		public static void Load(ContentManager content)
		{
			Player = content.Load<Texture2D>("Art/Player");
			Seeker = content.Load<Texture2D>("Art/Seeker");
			Wanderer = content.Load<Texture2D>("Art/Wanderer");
			Bullet = content.Load<Texture2D>("Art/Bullet");
			Lightning = content.Load<Texture2D>("Art/Lightning");
			Fireball = content.Load<Texture2D>("Art/Fireball");
			PoisonBall = content.Load<Texture2D>("Art/Poison_Ball");
			Pointer = content.Load<Texture2D>("Art/Pointer");
			BlackHole = content.Load<Texture2D>("Art/Black Hole");

			LineParticle = content.Load<Texture2D>("Art/Laser");
			Glow = content.Load<Texture2D>("Art/Glow");

			Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
			Pixel.SetData(new[] { Color.White });

			Font = content.Load<SpriteFont>("Font");
			
			LoadIcons(content);
		}

		private static void LoadIcons(ContentManager content) {
			LightningIcon = content.Load<Texture2D>("Art/Icons/Air_07_Call_Lightning");
			FireballIcon = content.Load<Texture2D>("Art/Icons/Fire_03_Meteor");
			PoisonIcon = content.Load<Texture2D>("Art/Icons/Nature_01_Missile");
			BurstIcon = content.Load<Texture2D>("Art/Icons/Light_08_Divine_Body");
			WindWallIcon = content.Load<Texture2D>("Art/Icons/Water_10_Aura_Of_Cold");
		}
		
		public static Texture2D LoadTexture(string assetName)
		{
			var texture = Texture2D.FromFile(WarlockGame.Instance.GraphicsDevice, assetName);
			return texture;
		}
	}
}