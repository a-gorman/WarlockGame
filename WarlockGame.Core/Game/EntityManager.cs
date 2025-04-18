using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game
{
	internal class EntityManager
	{
		private int _nextEntityId = 0;
		private readonly Dictionary<int, Entity> _entities = new();
		private readonly List<Projectile> _projectiles = new();
		/// <summary>
		/// Map between player Ids and warlocks
		/// </summary>
		private readonly Dictionary<int, Warlock> _warlocks = new();

		private bool _isUpdating;
		private readonly List<Entity> _addedEntities = new();

		public IReadOnlyCollection<Warlock> Warlocks => _warlocks.Values;

		public event Action<Warlock>? WarlockDestroyed;

		public void Add(Entity entity)
		{
			if (!_isUpdating)
				AddEntity(entity);
			else
				_addedEntities.Add(entity);
		}

		private void AddEntity(Entity entity) {
			entity.Id = _nextEntityId;
			_nextEntityId++;
			_entities.Add(entity.Id, entity);
			switch (entity) {
				case Projectile projectile:
					_projectiles.Add(projectile);
					break;
				case Warlock warlock:
					_warlocks.TryAdd(warlock.PlayerId, warlock);
					warlock.Destroyed += x => WarlockDestroyed?.Invoke(x);
					break;
			}
		}

		public void Update() {
			_isUpdating = true;
			HandleCollisions();

			foreach (var entity in _entities.Values)
				entity.Update();

			_isUpdating = false;

			foreach (var entity in _addedEntities)
				AddEntity(entity);

			_addedEntities.Clear();

			_entities.RemoveAll((_,x) => x.IsExpired);
			_projectiles.RemoveAll(x => x.IsExpired);
			_warlocks.RemoveAll((_, v) => v.IsExpired);
		}

		public Warlock? GetWarlockByPlayerId(int id) {
			_warlocks.TryGetValue(id, out var warlock);
			return warlock;
		}

		private void HandleCollisions() {
			foreach (var projectile in _projectiles) {
				_warlocks.Values.Where(x => x != projectile.Context.Caster)
				         .Concat<Entity>(_projectiles.Where(x => x != projectile))
				         .Where(x => IsColliding(projectile, x))
				         .ForEach(_ => projectile.OnCollision());
			}
		}

		private static bool IsColliding(Entity a, Entity b)
		{
			float radius = a.Radius + b.Radius;
			return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius.Squared();
		}
		
		/// <summary>
		/// Get's entities the given rectangle. Does not check for exact collision
		/// </summary>
		/// <param name="rectangle">The rectangle to check near</param>
		/// <returns>Entities near the given rectangle</returns>
		public IEnumerable<Entity> GetNearbyEntities(Rectangle rectangle)
		{
			// Account for radius (reject by assuming rxr rect)
			return _entities.Values.Where(x => {
				var boundingBox = new Rectangle((x.Position - new Vector2(x.Radius, x.Radius)).ToPoint(), new Point((int)x.Radius * 2, (int)x.Radius  * 2));
				return rectangle.Intersects(boundingBox);
			});
		}
		
		public IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
		{
			return _entities.Values.Where(x => Vector2.DistanceSquared(position, x.Position) < (x.Radius + radius) * (x.Radius + radius));
		}
		
		public Entity? GetEntity(int id)
		{
			_entities.TryGetValue(id, out var entity);
			return entity;
		}


		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var entity in _entities.Values)
				entity.Draw(spriteBatch);
		}

		public void Clear() {
			_entities.Clear();
			_projectiles.Clear();
			_addedEntities.Clear();
			_warlocks.Clear();
		}
	}
}