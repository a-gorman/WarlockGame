//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Display;
using NeonShooter.Core.Game.Graphics;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Entity
{
	internal class BlackHole : EntityBase
	{
		private static readonly Random _rand = new();

		private int _hitpoints = 10;
		private float _sprayAngle = 0;

		public BlackHole(Vector2 position) : base(new Sprite(Art.BlackHole))
		{
			Position = position;
			Radius = Art.BlackHole.Width / 2f;
		}

		public override void Update()
		{
			var entities = EntityManager.GetNearbyEntities(Position, 250);

			foreach (var entity in entities)
			{
				switch (entity)
				{
					case Enemy { IsActive: false }:
						continue;
					// bullets are repelled by black holes and everything else is attracted
					case Bullet:
						entity.Velocity += (entity.Position - Position).ScaleTo(0.3f);
						break;
					default:
					{
						var dPos = Position - entity.Position;
						var length = dPos.Length();

						entity.Velocity += dPos.ScaleTo(MathHelper.Lerp(2, 0, length / 250f));
						break;
					}
				}
			}

			// The black holes spray some orbiting particles. The spray toggles on and off every quarter second.
			if ((NeonShooterGame.GameTime.TotalGameTime.Milliseconds / 250) % 2 == 0)
			{
				Vector2 sprayVel = MathUtil.FromPolar(_sprayAngle, _rand.NextFloat(12, 15));
				Color color = ColorUtil.HsvToColor(5, 0.5f, 0.8f);	// light purple
				Vector2 pos = Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + _rand.NextVector2(4, 8);
				var state = new ParticleState() 
				{ 
					Velocity = sprayVel, 
					LengthMultiplier = 1, 
					Type = ParticleType.Enemy 
				};

				NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, pos, color, 190, 1.5f, state);
			}
			
			// rotate the spray direction
			_sprayAngle -= MathHelper.TwoPi / 50f;

			NeonShooterGame.Grid.ApplyImplosiveForce((float)Math.Sin(_sprayAngle / 2) * 10 + 20, Position, 200);
		}

		public void WasShot()
		{
			_hitpoints--;
			if (_hitpoints <= 0)
			{
				IsExpired = true;
				PlayerManager.ActivePlayer.Status.AddPoints(5);
				PlayerManager.ActivePlayer.Status.IncreaseMultiplier();
			}

			
			float hue = (float)((3 * NeonShooterGame.GameTime.TotalGameTime.TotalSeconds) % 6);
			Color color = ColorUtil.HsvToColor(hue, 0.25f, 1);
			const int numParticles = 150;
			float startOffset = _rand.NextFloat(0, MathHelper.TwoPi / numParticles);

			for (int i = 0; i < numParticles; i++)
			{
				Vector2 sprayVel = MathUtil.FromPolar(MathHelper.TwoPi * i / numParticles + startOffset, _rand.NextFloat(8, 16));
				Vector2 pos = Position + 2f * sprayVel;
				var state = new ParticleState() 
				{ 
					Velocity = sprayVel, 
					LengthMultiplier = 1, 
					Type = ParticleType.IgnoreGravity 
				};

				NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, pos, color, 90, 1.5f, state);
			}

			Sound.Explosion.Play(0.5f, _rand.NextFloat(-0.2f, 0.2f), 0);
		}

		public void Kill()
		{
			_hitpoints = 0;
			WasShot();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			// make the size of the black hole pulsate
			float scale = 1 + 0.1f * (float)Math.Sin(10 * NeonShooterGame.GameTime.TotalGameTime.TotalSeconds);
			_sprite.Draw(spriteBatch, Position, Orientation);
		}
	}
}