using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities
{
	class Entity
	{
		public int Id { get; set; }
		public int? PlayerId { get; set; }
		protected readonly Sprite _sprite;
		protected readonly Simulation _simulation;

		public CollisionType CollisionType { get; }
		public BoundingRectangle BoundingRectangle { get; private set; }
		public bool BlocksProjectiles { get; set; }
		public OrientedRectangle OrientedRectangle { get; private set; }	// used for polygon and rotated rectangle collision detection
		public float Radius { get; }						// used for circular collision detection
		public Vector2 Velocity { get; set; }
		public bool IsExpired { get; set; }					// true if the entity was destroyed and should be deleted.
		private List<Behavior> Behaviors { get; } = [];
		public event Action<OnDamagedEventArgs>? OnDamaged;
		public event Action<OnPushedEventArgs>? OnPushed;
		public event Action<OnCollisionEventArgs>? OnCollision;
		public List<CollisionFilter> CollisionFilters { get; } = new();
		
		public Vector2 Position {
			get => BoundingRectangle.Center;
			set {
				if (CollisionType == CollisionType.OrientedRectangle) {
					var translation = Matrix3x2.CreateTranslation(value - Position);
					OrientedRectangle = OrientedRectangle.Transform(OrientedRectangle, ref translation);
				}
				var bounds = BoundingRectangle;
				bounds.Center = value;
				BoundingRectangle = bounds;
			}
		}
		
		public float Orientation { 
			get;
			set {
				if (CollisionType == CollisionType.OrientedRectangle) {
					var rotation = Matrix3x2.CreateRotationZ(value - field);
					OrientedRectangle = OrientedRectangle.Transform(OrientedRectangle, ref rotation);
					BoundingRectangle = OrientedRectangle.BoundingRectangle;
				}
				field = value;
			}
		}
		
		/// <summary>
		/// Constructs an entity with a circle collision
		/// </summary>
		public Entity(Sprite sprite, Vector2 position, Simulation simulation, float radius = 20) {
			_sprite = sprite;
			_simulation = simulation;
			CollisionType = CollisionType.Circle;
			Radius = radius;
			BoundingRectangle = new CircleF(position, radius).BoundingRectangle;
		}

		/// <summary>
		/// Constructs an entity with an orientable rectangle collision
		/// </summary>
		public Entity(Sprite sprite, Vector2 center, float width, float height, float orientation, Simulation simulation) {
			_sprite = sprite;
			_simulation = simulation;
			Orientation = orientation;
			
			CollisionType = CollisionType.OrientedRectangle;
			OrientedRectangle = new OrientedRectangle(center, new SizeF(width, height), Matrix3x2.CreateRotationZ(orientation));
			BoundingRectangle = OrientedRectangle.BoundingRectangle;
		}

		public virtual void Update() {
			foreach (var behavior in Behaviors) {
				behavior.Update(this);
				if (behavior.IsExpired) {
					behavior.OnRemove(this);
				}
			}

			Behaviors.RemoveAll(x => x.IsExpired);
		}

		public void AddBehaviors(params Behavior[] behaviors) {
			foreach (var behavior in behaviors) {
				behavior.OnAdd(this);
				Behaviors.Add(behavior);
			}
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			_sprite.Draw(spriteBatch, Position, Orientation);
		}

		public virtual void Damage(float damage, Entity source) {
			if (OnDamaged != null) {
				var args = new OnDamagedEventArgs() {
					Amount = damage,
					Source = this,
					DamageSource = source
				};
				
				OnDamaged.Invoke(args);
			}
		}

		public virtual void HandleCollision(Entity other) {
			OnCollision?.Invoke(new OnCollisionEventArgs { Source = this, Other = other });
		}
		
		public void Push(Vector2 forceVector) {
			OnPushed?.Invoke(new OnPushedEventArgs { Source = this, Force = forceVector.NanToZero() });
		}
	}

	delegate bool CollisionFilter(Entity source, Entity other);
	
	enum CollisionType {
		None,
		Circle,
		Rectangle,
		OrientedRectangle
	}
}