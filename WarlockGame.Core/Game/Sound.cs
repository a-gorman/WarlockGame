using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace WarlockGame.Core.Game
{
	internal static class Sound
	{
		private static readonly Random Rand = new();
		public static GameSound None = null!;

		public static GameSound Lightning = null!;

		public static void Load(ContentManager content)
		{
			Lightning = new GameSound(content.Load<SoundEffect>("Audio/wav_Thunder_Spell_Shoot_6"));
			None = new GameSound(null!) { Disabled = true };
		}
	}
	
	public class GameSound(SoundEffect soundEffect) {
		public bool Disabled { get; set; } = false;
		
		public void Play() {
			if(!Disabled)
				soundEffect.Play(volume: Configuration.Volume, 0f, 0f);
		}
	}
}