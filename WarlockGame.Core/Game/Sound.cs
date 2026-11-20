using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace WarlockGame.Core.Game
{
	internal static class Sound
	{
		private static readonly Random Rand = new();
		public static GameSound None = null!;

		public static GameSound Lightning = null!;
		public static GameSound FireHit = null!;
		public static GameSound FireBallCast = null!;
		public static GameSound FireSprayCast = null!;
		public static GameSound WindwallLoop = null!;
		public static GameSound Swap = null!;

		public static void Load(ContentManager content)
		{
			Lightning = new GameSound(content.Load<SoundEffect>("Audio/thunder_spell_shoot_6"));
			FireHit = new GameSound(content.Load<SoundEffect>("Audio/fire_spell_hit_1"));
			FireBallCast = new GameSound(content.Load<SoundEffect>("Audio/fire_spell_hit_6"));
			FireSprayCast = new GameSound(content.Load<SoundEffect>("Audio/fire_spell_shoot_9_edited"));
			FireSprayCast = new GameSound(content.Load<SoundEffect>("Audio/fire_spell_shoot_9_edited"));
			Swap = new GameSound(content.Load<SoundEffect>("Audio/Effects/Portal_Opening_Sound_40"));
			WindwallLoop = new GameSound(content.Load<SoundEffect>("Audio/Effects/wind_spell_loop_3"), volume: 0.5f);
			None = new GameSound(null!);
		}
	}
	
	public class GameSound(SoundEffect? soundEffect, float volume = 1f) {
		public void Play() {
			if(soundEffect != null && !(Configuration.MuteWhenNotFocused && !WarlockGame.Instance.IsActive))
				soundEffect.Play(volume: Configuration.Volume * volume, 0f, 0f);
		}
		
		public void Play(Vector2 location) {
			if(soundEffect != null && WarlockGame.Instance.IsPointOnScreen(location) 
			             && !(Configuration.MuteWhenNotFocused && !WarlockGame.Instance.IsActive))
				soundEffect.Play(volume: Configuration.Volume * volume, 0f, 0f);
		}
		
		public void Play(Vector2 location, float radius) {
			if(soundEffect != null && WarlockGame.Instance.IsPointWithinDistanceOfScreen(location, radius) 
			                        && !(Configuration.MuteWhenNotFocused && !WarlockGame.Instance.IsActive))
				soundEffect.Play(volume: Configuration.Volume * volume, 0f, 0f);
		}
	}
}