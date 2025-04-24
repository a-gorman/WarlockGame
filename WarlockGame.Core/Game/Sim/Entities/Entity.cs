using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;

namespace WarlockGame.Core.Game.Sim.Entities
{
	class Entity
	{
		public int Id { get; set; }
		protected readonly Sprite _sprite;
		protected readonly Simulation _simulation;

		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Orientation { get; set; }
		public float Radius { get; init; } = 20;	// used for circular collision detection
		public bool IsExpired { get; set; }			// true if the entity was destroyed and should be deleted.
		private List<Behavior> Behaviors { get; } = [];
		public event Action<OnDamagedEventArgs>? OnDamaged;
		public event Action<OnPushedEventArgs>? OnPushed;
		public event Action<OnCollisionEventArgs>? OnCollision;
		
		public Entity(Sprite sprite, Simulation simulation) {
			_sprite = sprite;
			_simulation = simulation;
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
			if(OnCollision != null) {
				var args = new OnCollisionEventArgs() {
					Source = this,
					Other = other
				};

				OnCollision.Invoke(args);
			}
		}
		
		public void Push(Vector2 forceVector) {
			OnPushed?.Invoke(new OnPushedEventArgs { Source = this, Force = forceVector });
		}
	}
}