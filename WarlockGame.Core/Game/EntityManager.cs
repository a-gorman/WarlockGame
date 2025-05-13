using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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
			var entities = _entities.Values.ToList();
			for (int i = 0; i < entities.Count - 1; i++) {
				for (int j = i + 1; j < entities.Count; j++) {
					if (IsColliding(entities[i], entities[j])) {
						entities[i].HandleCollision(entities[j]);
						entities[j].HandleCollision(entities[i]);
					}
				}
			}
		}

		private static bool IsColliding(Entity a, Entity b) {
			if (a.CollisionType == CollisionType.None || b.CollisionType == CollisionType.None) {
				return false;
			}

			if (!a.BoundingRectangle.Intersects(b.BoundingRectangle)) {
				return false;
			}

			switch (a.CollisionType) {
				case CollisionType.Circle:
					switch (b.CollisionType) {
						case CollisionType.Circle:
							float radius = a.Radius + b.Radius;
							return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius.Squared();
						case CollisionType.Rectangle:
							return new CircleF(a.Position, a.Radius).Intersects(b.BoundingRectangle);
						case CollisionType.OrientedRectangle:
							return a.OrientedRectangle.Intersects(new CircleF(b.Position, b.Radius));
					}
					break;
				case CollisionType.Rectangle:
					switch (b.CollisionType) {
						case CollisionType.Circle:
							return new CircleF(b.Position, b.Radius).Intersects(a.BoundingRectangle);
						case CollisionType.Rectangle:
							return true;
						case CollisionType.OrientedRectangle:
							var bounds = a.BoundingRectangle;
							var rect = RectangleF.CreateFrom(bounds.Center - bounds.HalfExtents * 2,
								bounds.Center + bounds.HalfExtents * 2);
							return b.OrientedRectangle.Intersects(rect);
					}
					break;
				case CollisionType.OrientedRectangle:
					switch (b.CollisionType) {
						case CollisionType.Circle:
							return Util.Geometry.IsColliding(b.Position, b.Radius, a.BoundingRectangle, new Angle(a.Orientation));
						case CollisionType.Rectangle:
							var bounds = a.BoundingRectangle;
							var rect = RectangleF.CreateFrom(bounds.Center - bounds.HalfExtents * 2,
								bounds.Center + bounds.HalfExtents * 2);
							return b.OrientedRectangle.Intersects(rect);
						case CollisionType.OrientedRectangle:
							return a.OrientedRectangle.Intersects(b.OrientedRectangle);
					}
					break;
			}

			return false;
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