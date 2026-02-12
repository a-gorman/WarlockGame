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
		public static Texture2D Triple { get; private set; } = null!;
		public static Texture2D EnergySpark { get; private set; } = null!;
		public static Texture2D PowerBall { get; private set; } = null!;
		public static Texture2D Pointer { get; private set; } = null!;
		public static Texture2D BlackHole { get; private set; } = null!;

		public static Texture2D FlameStrike { get; private set; } = null!;

		public static Texture2D LineParticle { get; private set; } = null!;
		public static Texture2D Glow { get; private set; } = null!;
		public static Texture2D Pixel { get; private set; } = null!;		// a single white pixel

		public static SpriteFont Font { get; private set; } = null!;
		
		public static Texture2D FireballIcon { get; private set; } = null!;
		public static Texture2D LightningIcon { get; private set; } = null!;
		public static Texture2D PoisonIcon { get; private set; } = null!;
		public static Texture2D BurstIcon { get; private set; } = null!;
		public static Texture2D WindWallIcon { get; private set; } = null!;
		public static Texture2D BoomerangIcon { get; set; } = null!;
		public static Texture2D LightStrikeIcon { get; set; } = null!;
		public static Texture2D HomingIcon { get; set; } = null!;
		public static Texture2D RefractionShieldIcon { get; set; } = null!;
		public static Texture2D SoulShatterIcon { get; set; } = null!;
		public static Texture2D HealIcon { get; set; } = null!;
		public static Texture2D InvisibilityIcon { get; set; } = null!;
		public static Texture2D DefianceIcon { get; set; } = null!;
		public static Texture2D BigExplosionIcon { get; set; } = null!;
		public static Texture2D ReducedBoundaryDamageIcon { get; set; } = null!;
		public static Texture2D ReducedAllDamageIcon { get; set; } = null!;
		public static Texture2D FireSprayIcon { get; set; } = null!;

		public static void Load(ContentManager content)
		{
			Player = content.Load<Texture2D>("Art/Player");
			Seeker = content.Load<Texture2D>("Art/Seeker");
			Wanderer = content.Load<Texture2D>("Art/Wanderer");
			Bullet = content.Load<Texture2D>("Art/Bullet");
			Lightning = content.Load<Texture2D>("Art/Lightning");
			Fireball = content.Load<Texture2D>("Art/Fireball");
			PoisonBall = content.Load<Texture2D>("Art/Poison_Ball");
			EnergySpark = content.Load<Texture2D>("Art/Energy_Spark");
			PowerBall = content.Load<Texture2D>("Art/Power_Ball");
			Triple = content.Load<Texture2D>("Art/Triple");
			Pointer = content.Load<Texture2D>("Art/Pointer");
			BlackHole = content.Load<Texture2D>("Art/Black Hole");

			LineParticle = content.Load<Texture2D>("Art/Laser");
			Glow = content.Load<Texture2D>("Art/Glow");

			FlameStrike = content.Load<Texture2D>("Art/flame_strike");

			Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
			Pixel.SetData(new[] { Color.White });

			Font = content.Load<SpriteFont>("Font");
			
			LoadIcons(content);
		}

		private static void LoadIcons(ContentManager content) {
			LightningIcon = LoadIcon(content, "Air_07_Call_Lightning");
			FireballIcon = LoadIcon(content, "Fire_03_Meteor");
			PoisonIcon = LoadIcon(content, "Nature_01_Missile");
			BurstIcon = LoadIcon(content, "warlock_skill_43");
			WindWallIcon = LoadIcon(content, "Water_10_Aura_Of_Cold");
			SoulShatterIcon = LoadIcon(content, "General_15_Blur");
			RefractionShieldIcon = LoadIcon(content, "Light_05_Bouncing_Light");
			HomingIcon = LoadIcon(content, "Water_08_Ball_Of_Water");
			BoomerangIcon = LoadIcon(content, "paladin_skill_24");
			LightStrikeIcon = LoadIcon(content, "rpg_skill_71");
			FireSprayIcon = LoadIcon(content, "pyromancer_11");
			
			HealIcon = LoadIcon(content, "General_02_Heal_T");
			InvisibilityIcon = LoadIcon(content, "Invis");
			DefianceIcon = LoadIcon(content, "Big_Explosion");
			BigExplosionIcon = LoadIcon(content, "Defiance");
			ReducedBoundaryDamageIcon = LoadIcon(content, "rpg_skill_42");
			ReducedAllDamageIcon = LoadIcon(content, "paladin_skill_34");
		}

		private static Texture2D LoadIcon(ContentManager content, string iconName) {
			return content.Load<Texture2D>($"Art/Icons/{iconName}");
		}

		public static Texture2D LoadTexture(string assetName)
		{
			var texture = Texture2D.FromFile(WarlockGame.Instance.GraphicsDevice, assetName);
			return texture;
		}
	}
}