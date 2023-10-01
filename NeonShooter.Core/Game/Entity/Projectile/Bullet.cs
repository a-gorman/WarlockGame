//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Display;
using NeonShooter.Core.Game.Graphics;
using NeonShooter.Core.Game.Projectile;

namespace NeonShooter.Core.Game
{
	internal class Bullet : Entity.EntityBase, IProjectile
	{
		private static readonly Random _rand = new();

		public Bullet(Vector2 position, Vector2 velocity) :
			base(new Sprite(Art.Bullet))
		{
			Position = position;
			Velocity = velocity;
			Orientation = Velocity.ToAngle();
			Radius = 8;
		}

		public override void Update()
		{
			if (Velocity.LengthSquared() > 0)
				Orientation = Velocity.ToAngle();

			Position += Velocity;
			NeonShooterGame.Grid.ApplyExplosiveForce(0.5f * Velocity.Length(), Position, 80);

			// delete bullets that go off-screen
			if (!NeonShooterGame.Viewport.Bounds.Contains(Position.ToPoint()))
			{
				IsExpired = true;

				for (int i = 0; i < 30; i++)
					NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
						new ParticleState() { Velocity = _rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });

			}
		}

		public void OnHit()
		{
			IsExpired = true;
		}
	}
}