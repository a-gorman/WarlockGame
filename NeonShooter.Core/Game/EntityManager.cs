//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Projectile;

namespace NeonShooter.Core.Game
{
	internal static class EntityManager
	{
		private static List<Entity> _entities = new();
		private static List<Enemy> _enemies = new();
		private static List<IProjectile> _projectiles = new();
		private static List<BlackHole> _blackHoles = new();

		public static IEnumerable<BlackHole> BlackHoles => _blackHoles;

		private static bool _isUpdating;
		private static readonly List<Entity> _addedEntities = new();

		public static int Count => _entities.Count;
		public static int BlackHoleCount => _blackHoles.Count;

		public static void Add(Entity entity)
		{
			if (!_isUpdating)
				AddEntity(entity);
			else
				_addedEntities.Add(entity);
		}

		private static void AddEntity(Entity entity)
		{
			_entities.Add(entity);
			if (entity is IProjectile projectile)
				_projectiles.Add(projectile);
			else if (entity is Enemy enemy)
				_enemies.Add(enemy);
			else if (entity is BlackHole hole)
				_blackHoles.Add(hole);
		}

		public static void Update()
		{
			_isUpdating = true;
			HandleCollisions();

			foreach (var entity in _entities)
				entity.Update();

			_isUpdating = false;

			foreach (var entity in _addedEntities)
				AddEntity(entity);

			_addedEntities.Clear();

			_entities = _entities.Where(x => !x.IsExpired).ToList();
			_projectiles = _projectiles.Where(x => !x.IsExpired).ToList();
			_enemies = _enemies.Where(x => !x.IsExpired).ToList();
			_blackHoles = _blackHoles.Where(x => !x.IsExpired).ToList();
		}

		private static void HandleCollisions()
		{
			HandleEnemyEnemyCollisions();
			HandleEnemyBulletCollisions();
			HandlePlayerEnemyCollisions();
			HandleBlackHoleCollisions();
		}

		private static void HandleEnemyEnemyCollisions()
		{
			for (int i = 0; i < _enemies.Count; i++)
			{
				for (int j = i + 1; j < _enemies.Count; j++)
				{
					if (IsColliding(_enemies[i], _enemies[j]))
					{
						_enemies[i].HandleCollision(_enemies[j]);
						_enemies[j].HandleCollision(_enemies[i]);
					}
				}
			}
		}

		private static void HandlePlayerEnemyCollisions()
		{
			if (_enemies.Any(x => x.IsActive && IsColliding(x, PlayerShip.Instance)))
			{
				KillPlayer();
			}
		}

		private static void HandleEnemyBulletCollisions()
		{
			foreach (var enemy in _enemies)
			{
				foreach (var bullet in _projectiles.Where(x => IsColliding(enemy, x)))
				{
					// enemy.WasShot();
					bullet.OnHit();
				}
			}
		}

		private static void HandleBlackHoleCollisions()
		{
			foreach (var blackHole in _blackHoles)
			{
				foreach (var enemy in _enemies)
					if (enemy.IsActive && IsColliding(blackHole, enemy))
						enemy.WasShot();

				foreach (var bullet in _projectiles)
				{
					if (IsColliding(blackHole, bullet))
					{
						bullet.IsExpired = true;
						blackHole.WasShot();
					}
				}

				if (IsColliding(PlayerShip.Instance, blackHole))
				{
					KillPlayer();
					break;
				}
			}
		}

		private static void KillPlayer()
		{
			PlayerShip.Instance.Kill();
			_enemies.ForEach(x => x.WasShot());
			_blackHoles.ForEach(x => x.Kill());
			EnemySpawner.Reset();
		}

		private static bool IsColliding(IEntity a, IEntity b)
		{
			float radius = a.Radius + b.Radius;
			return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
		}

		public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
		{
			return _entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius);
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			foreach (var entity in _entities)
				entity.Draw(spriteBatch);
		}
	}
}