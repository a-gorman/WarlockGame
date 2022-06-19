//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter
{
	internal static class EntityManager
	{
		private static List<Entity> _entities = new List<Entity>();
		private static List<Enemy> _enemies = new List<Enemy>();
		private static List<Bullet> _bullets = new List<Bullet>();
		private static List<BlackHole> _blackHoles = new List<BlackHole>();

		public static IEnumerable<BlackHole> BlackHoles { get { return _blackHoles; } }

		private static bool _isUpdating;
		private static readonly List<Entity> _addedEntities = new List<Entity>();

		public static int Count { get { return _entities.Count; } }
		public static int BlackHoleCount { get { return _blackHoles.Count; } }

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
			if (entity is Bullet)
				_bullets.Add(entity as Bullet);
			else if (entity is Enemy)
				_enemies.Add(entity as Enemy);
			else if (entity is BlackHole)
				_blackHoles.Add(entity as BlackHole);
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
			_bullets = _bullets.Where(x => !x.IsExpired).ToList();
			_enemies = _enemies.Where(x => !x.IsExpired).ToList();
			_blackHoles = _blackHoles.Where(x => !x.IsExpired).ToList();
		}

		private static void HandleCollisions()
		{
			// handle collisions between enemies
			for (int i = 0; i < _enemies.Count; i++)
				for (int j = i + 1; j < _enemies.Count; j++)
				{
					if (IsColliding(_enemies[i], _enemies[j]))
					{
						_enemies[i].HandleCollision(_enemies[j]);
						_enemies[j].HandleCollision(_enemies[i]);
					}
				}

			// handle collisions between bullets and enemies
			for (int i = 0; i < _enemies.Count; i++)
				for (int j = 0; j < _bullets.Count; j++)
				{
					if (IsColliding(_enemies[i], _bullets[j]))
					{
						_enemies[i].WasShot();
						_bullets[j].IsExpired = true;
					}
				}

			// handle collisions between the player and enemies
			for (int i = 0; i < _enemies.Count; i++)
			{
				if (_enemies[i].IsActive && IsColliding(PlayerShip.Instance, _enemies[i]))
				{
					KillPlayer();
					break;
				}
			}

			// handle collisions with black holes
			for (int i = 0; i < _blackHoles.Count; i++)
			{
				for (int j = 0; j < _enemies.Count; j++)
					if (_enemies[j].IsActive && IsColliding(_blackHoles[i], _enemies[j]))
						_enemies[j].WasShot();

				for (int j = 0; j < _bullets.Count; j++)
				{
					if (IsColliding(_blackHoles[i], _bullets[j]))
					{
						_bullets[j].IsExpired = true;
						_blackHoles[i].WasShot();
					}
				}

				if (IsColliding(PlayerShip.Instance, _blackHoles[i]))
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

		private static bool IsColliding(Entity a, Entity b)
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