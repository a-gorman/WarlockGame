using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Util;
using WarlockGame.Core.Game.Entity;

namespace WarlockGame.Core.Game
{
	internal static class EntityManager
	{
		private static List<EntityBase> _entities = new();
		private static List<Projectile> _projectiles = new();
		/// <summary>
		/// Map between player Ids and warlocks
		/// </summary>
		private static Dictionary<int, Warlock> _warlocks = new();

		private static bool _isUpdating;
		private static readonly List<EntityBase> _addedEntities = new();

		public static int Count => _entities.Count;
		public static IReadOnlyCollection<Warlock> Warlocks => _warlocks.Values;

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
			switch (entity) {
				case Projectile projectile:
					_projectiles.Add(projectile);
					break;
				case Warlock warlock:
					_warlocks.TryAdd(warlock.PlayerId, warlock);
					break;
			}
		}

		public static void Update() {
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
		}

		public static Warlock? GetWarlockByPlayerId(int id) {
			_warlocks.TryGetValue(id, out var warlock);
			return warlock;
		}

		private static void HandleCollisions() {
			foreach (var projectile in _projectiles) {
				_entities.OfType<Warlock>().Where(x => x != projectile.Caster)
				         .Concat<IEntity>(_projectiles.Where(x => x != projectile))
				         .Where(x => IsColliding(projectile, x))
				         .ForEach(_ => projectile.OnCollision());
			}
		}

		private static bool IsColliding(IEntity a, IEntity b)
		{
			float radius = a.Radius + b.Radius;
			return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius.Squared();
		}
		
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

		public static void Clear() {
			_entities.Clear();
			_projectiles.Clear();
			_addedEntities.Clear();
			_warlocks.Clear();
		}
	}
}