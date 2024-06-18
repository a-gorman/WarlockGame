//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace WarlockGame.Core.Game
{
	internal static class Sound
	{
		public static Song Music { get; private set; }

		private static readonly Random Rand = new();

		private static SoundEffect[] _explosions;
		// return a random explosion sound
		public static SoundEffect Explosion => _explosions[Rand.Next(_explosions.Length)];

		private static SoundEffect[] _shots;
		public static SoundEffect Shot => _shots[Rand.Next(_shots.Length)];

		private static SoundEffect[] _spawns;
		public static SoundEffect Spawn => _spawns[Rand.Next(_spawns.Length)];

		public static void Load(ContentManager content)
		{
			Music = content.Load<Song>("Audio/Music");

			// These linq expressions are just a fancy way loading all sounds of each category into an array.
			_explosions = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Audio/explosion-0" + x)).ToArray();
			_shots = Enumerable.Range(1, 4).Select(x => content.Load<SoundEffect>("Audio/shoot-0" + x)).ToArray();
			_spawns = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Audio/spawn-0" + x)).ToArray();
		}
	}
}