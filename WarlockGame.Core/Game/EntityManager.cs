using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Entities;
using WarlockGame.Core.Game.Util;
using ZLinq;

namespace WarlockGame.Core.Game
{
	internal class EntityManager
	{
		private int _nextEntityId = 1;
		private readonly Dictionary<int, Entity> _entities = new();
		private readonly List<Projectile> _projectiles = new();
		/// <summary>
		/// Map between player Ids and warlocks. Contains dead warlocks!
		/// </summary>
		private readonly Dictionary<int, Warlock> _warlocksLivingOrDead = new();

		private readonly HashSet<Warlock> _livingWarlocks = new();

		private bool _isUpdating;
		private readonly List<Entity> _addedEntities = new();

		public IReadOnlyCollection<Warlock> Warlocks => _livingWarlocks;
		public IReadOnlyCollection<Entity> EntitiesLivingOrDead => _entities.Values;

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
					if (_warlocksLivingOrDead.TryAdd(warlock.PlayerId!.Value, warlock)) {
						_livingWarlocks.Add(warlock);
						warlock.Destroyed += OnWarlockDestroyed;
					}
					break;
			}
		}

		public void Update() {
			_isUpdating = true;
			HandleCollisions();

			foreach (var entity in _entities.Values.AsValueEnumerable().Where(x => !x.IsDead))
				entity.Update();

			_isUpdating = false;

			foreach (var entity in _addedEntities)
				AddEntity(entity);

			_addedEntities.Clear();

			_entities.RemoveAll((_,x) => x.IsExpired);
			_projectiles.RemoveAll(x => x.IsExpired);
			_warlocksLivingOrDead.RemoveAll((_, v) => v.IsExpired);
		}

		public Warlock? GetWarlockByForceId(int forceId) {
			if (_warlocksLivingOrDead.TryGetValue(forceId, out var warlock) && !warlock.IsDead) {
				return warlock;
			}
			return null;
		}
		
		public bool TryGetWarlockByForceId(int forceId, out Warlock? warlock) {
			if (_warlocksLivingOrDead.TryGetValue(forceId, out warlock) && !warlock.IsDead) {
				return true;
			}

			warlock = null;
			return false;
		}
		
		/// <summary>
		/// Gets a warlock by their force's id.
		/// Will also get dead warlocks, which is useful for "metagame" functionality
		/// </summary>
		public Warlock? GetWarlockLivingOrDeadByForceId(int forceId) {
			_warlocksLivingOrDead.TryGetValue(forceId, out var warlock);
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

			if (a.IsDead || b.IsDead) {
				return false;
			}

			if (!a.BoundingRectangle.Intersects(b.BoundingRectangle)) {
				return false;
			}

			foreach (var filter in a.CollisionFilters.AsValueEnumerable()) {
				if (!filter.Invoke(a, b)) {
					return false;
				}
			}
			
			foreach (var filter in b.CollisionFilters.AsValueEnumerable()) {
				if (!filter.Invoke(b, a)) {
					return false;
				}
			}

			switch (a.CollisionType) {
				case CollisionType.Circle:
					switch (b.CollisionType) {
						case CollisionType.Circle:
							float radius = a.Radius + b.Radius;
							return Vector2.DistanceSquared(a.Position, b.Position) < radius.Squared();
						case CollisionType.Rectangle:
							return new CircleF(a.Position, a.Radius).Intersects(b.BoundingRectangle);
						case CollisionType.OrientedRectangle:
							return b.OrientedRectangle.Intersects(new CircleF(a.Position, a.Radius));
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
							return a.OrientedRectangle.Intersects(new CircleF(b.Position, b.Radius));
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
			return _entities.Values
				.Where(x => !x.IsDead && Vector2.DistanceSquared(position, x.Position) < (x.Radius + radius) * (x.Radius + radius));
		}
		
		public Entity? GetEntity(int id)
		{
			_entities.TryGetValue(id, out var entity);
			if (entity != null && !entity.IsDead) {
				return entity;
			}
			return null;
		}



		public void Clear() {
			_entities.Clear();
			_projectiles.Clear();
			_addedEntities.Clear();
			_warlocksLivingOrDead.Clear();
			_livingWarlocks.Clear();
		}

		private void OnWarlockDestroyed(Warlock warlock) {
			if (_livingWarlocks.Remove(warlock)) {
				WarlockDestroyed?.Invoke(warlock);
			}
			else {
				Logger.Warning($"Tried to destroy warlock more than once. Id: {warlock.Id}", Logger.LogType.Simulation);
			}
		}

		public void RespawnWarlock(int forceId, Vector2 location) {
			if (_warlocksLivingOrDead.TryGetValue(forceId, out var warlock)) {
				if (!_livingWarlocks.Contains(warlock)) {
					warlock.Position = location;
					warlock.Respawn();
					_livingWarlocks.Add(warlock);
				}
				else {
					Logger.Warning($"Tried to respawn a warlock that is already alive for force {forceId}", Logger.LogType.Simulation);
				}
			}
			else {
				Logger.Warning($"Tried to respawn warlock for force that does not exist! Id: {forceId}", Logger.LogType.Simulation);
			}
		}
	}
}