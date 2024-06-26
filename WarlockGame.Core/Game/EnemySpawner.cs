﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game
{
	internal static class EnemySpawner
	{
		private static readonly Random _rand = new();
		private static float _inverseSpawnChance = 90;
		private static readonly float _inverseBlackHoleChance = 600;

		private static Warlock PlayerInstance => PlayerManager.Players.First().Warlock;

		
		public static void Update()
		{
			if (!PlayerInstance.IsDead && EntityManager.Count < 200)
			{
				if (_rand.Next((int)_inverseSpawnChance) == 0)
					EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
				
				if (_rand.Next((int)_inverseSpawnChance) == 0)
					EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
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
				pos = new Vector2(_rand.Next((int)WarlockGame.ScreenSize.X), _rand.Next((int)WarlockGame.ScreenSize.Y));
			} 
			while (Vector2.DistanceSquared(pos, PlayerInstance.Position) < 250 * 250);

			return pos;
		}

		public static void Reset()
		{
			_inverseSpawnChance = 90;
		}
	}
}