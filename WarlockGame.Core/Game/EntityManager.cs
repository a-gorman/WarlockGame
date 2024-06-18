//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Util;
using WarlockGame.Core.Game.Entity;
using WarlockGame.Core.Game.Entity.Projectile;

namespace WarlockGame.Core.Game
{
	internal static class EntityManager
	{
		private static List<EntityBase> _entities = new();
		private static List<Enemy> _enemies = new();
		private static List<IProjectile> _projectiles = new();


		private static bool _isUpdating;
		private static readonly List<EntityBase> _addedEntities = new();

		public static int Count => _entities.Count;

		private static Warlock PlayerInstance => PlayerManager.Players.First().Warlock;
		
		public static void Add(EntityBase entity)
		{
			if (!_isUpdating)
				AddEntity(entity);
			else
				_addedEntities.Add(entity);
		}

		private static void AddEntity(EntityBase entity)
		{
			_entities.Add(entity);
			if (entity is IProjectile projectile)
				_projectiles.Add(projectile);
			else if (entity is Enemy enemy)
				_enemies.Add(enemy);
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
		}

		private static void HandleCollisions()
		{
			HandleEnemyEnemyCollisions();
			HandleBulletCollisions();
			HandlePlayerEnemyCollisions();
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
			if (_enemies.Any(x => x.IsActive && IsColliding(x, PlayerInstance)))
			{
				KillPlayer();
			}
		}

		private static void HandleBulletCollisions()
		{

			
			foreach (var projectile in _projectiles) {
				_enemies
					.Concat<IEntity>(_entities.OfType<Warlock>().Where(x => x != projectile.Parent))
					.Concat(_projectiles.Where(x => x != projectile))
					.Where(x => IsColliding(projectile, x))
					.ForEach(_ => projectile.OnHit());
			}
		}

		private static void KillPlayer()
		{
			PlayerInstance.Kill();
			_enemies.ForEach(x => x.WasShot());
			EnemySpawner.Reset();
		}

		private static bool IsColliding(IEntity a, IEntity b)
		{
			float radius = a.Radius + b.Radius;
			return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius.Squared();
		}
		
		// public static IEnumerable<EntityBase> GetEntitiesWithinRectangle(Vector2 position, int width, int height, float angle) {
		// 	var lineSegment = new LineSegment(position, new Vector2((position.X + width) * Single.Sin(angle), (position.Y + height) * Single.Sin(angle)));
		//
		// 	return _entities
		// 		// Reject
		// 		.Where(x => x.Position.IsWithin(lineSegment.BoundingBox) && ;
		// }

		/// <summary>
		/// Get's entities the given rectangle. Does not check for exact collision
		/// </summary>
		/// <param name="rectangle">The rectangle to check near</param>
		/// <returns>Entities near the given rectangle</returns>
		public static IEnumerable<EntityBase> GetNearbyEntities(Rectangle rectangle)
		{
			// Account for radius (reject by assuming rxr rect)
			return _entities.Where(x => {
				var boundingBox = new Rectangle((x.Position - new Vector2(x.Radius, x.Radius)).ToPoint(), new Point((int)x.Radius * 2, (int)x.Radius  * 2));
				return rectangle.Intersects(boundingBox);
			});
		}
		
		public static IEnumerable<EntityBase> GetNearbyEntities(Vector2 position, float radius)
		{
			return _entities.Where(x => Vector2.DistanceSquared(position, x.Position) < (x.Radius + radius) * (x.Radius + radius));
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			foreach (var entity in _entities)
				entity.Draw(spriteBatch);
		}
	}
}