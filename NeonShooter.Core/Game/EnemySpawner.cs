//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;

namespace NeonShooter.Core.Game
{
	internal static class EnemySpawner
	{
		private static readonly Random _rand = new();
		private static float _inverseSpawnChance = 90;
		private static readonly float _inverseBlackHoleChance = 600;

		public static void Update()
		{
			if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
			{
				if (_rand.Next((int)_inverseSpawnChance) == 0)
					EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
				
				if (_rand.Next((int)_inverseSpawnChance) == 0)
					EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
				
				if (EntityManager.BlackHoleCount < 2 && _rand.Next((int)_inverseBlackHoleChance) == 0)
					EntityManager.Add(new BlackHole(GetSpawnPosition()));
			}
			
			// slowly increase the spawn rate as time progresses
			if (_inverseSpawnChance > 30)
				_inverseSpawnChance -= 0.005f;
		}

		private static Vector2 GetSpawnPosition()
		{
			Vector2 pos;
			do
			{
				pos = new Vector2(_rand.Next((int)NeonShooterGame.ScreenSize.X), _rand.Next((int)NeonShooterGame.ScreenSize.Y));
			} 
			while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

			return pos;
		}

		public static void Reset()
		{
			_inverseSpawnChance = 90;
		}
	}
}