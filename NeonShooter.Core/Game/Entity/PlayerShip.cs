//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Graphics;
using NeonShooter.Core.Game.Spell;
using NeonShooter.Core.Game.Util;

namespace NeonShooter.Core.Game.Entity
{
    internal class PlayerShip : EntityBase
    {
        public static PlayerShip Instance { get; } = new();

        const float Speed = 8;

        public List<WarlockSpell> Spells { get; } = new() { SpellFactory.Fireball(), SpellFactory.Lightning() };

        private int _framesUntilRespawn = 0;
        public bool IsDead => _framesUntilRespawn > 0;

        private static readonly Random _rand = new();

        private PlayerShip() :
            base(new Sprite(Art.Player))
        {
            Position = NeonShooterGame.ScreenSize / 2;
            Radius = 10;
        }

        public override void Update() {
            if (IsDead) {
                if (--_framesUntilRespawn == 0) {
                    if (PlayerStatus.Lives == 0) {
                        PlayerStatus.Reset();
                        Position = NeonShooterGame.ScreenSize / 2;
                    }
                    NeonShooterGame.Grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(Position, 0), 50);
                }
                return;
            }

            Move();
            
            MakeExhaustFire();

            if (Input.WasRightMousePressed()) {
                CastSpell(Spells.First());
            }
            
            if (Input.WasLeftMousePressed()) {
                CastSpell(Spells.Skip(1).First());
            }
            
            foreach (var spell in Spells) {
                spell.Update();
            }
        }

        private void Move() {
            var inputDirection = Input.GetMovementDirection();

            if (Velocity.IsLengthLessThan(Speed)) {
                Velocity = Vector2.Zero;
            }
            else {
                Velocity -= Velocity.WithLength(Speed);
            }
            
            if (inputDirection.HasLength()) {
                Velocity += Speed * inputDirection;
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, _sprite.Size / 2, NeonShooterGame.ScreenSize - _sprite.Size / 2);

            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            Velocity -= Velocity.ToNormalizedOrZero() * 8;
        }

        private void CastSpell(WarlockSpell spell)
        {
            if (!spell.OnCooldown && Input.GetAimDirection(Position) is var aim && aim.HasLength())
            {
                spell.Cast(Position, aim);
            }
        }

        private void MakeExhaustFire()
        {
            if (Velocity.LengthSquared() > 0.1f)
            {
                // set up some variables
                Orientation = Extensions.ToAngle(Velocity);
                Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, Orientation);

                double t = NeonShooterGame.GameTime.TotalGameTime.TotalSeconds;
                // The primary velocity of the particles is 3 pixels/frame in the direction opposite to which the ship is travelling.
                Vector2 baseVel = Extensions.ScaleTo(Velocity, -3);
                // Calculate the sideways velocity for the two side streams. The direction is perpendicular to the ship's velocity and the
                // magnitude varies sinusoidally.
                Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
                Color sideColor = new Color(200, 38, 9); // deep red
                Color midColor = new Color(255, 187, 30); // orange-yellow
                Vector2 pos =
                    Position + Vector2.Transform(new Vector2(-25, 0), rot); // position of the ship's exhaust pipe.
                const float alpha = 0.7f;

                // middle particle stream
                Vector2 velMid = baseVel + _rand.NextVector2(0, 1);
                NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));
                NeonShooterGame.ParticleManager.CreateParticle(Art.Glow, pos, midColor * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));

                // side particle streams
                Vector2 vel1 = baseVel + perpVel + _rand.NextVector2(0, 0.3f);
                Vector2 vel2 = baseVel - perpVel + _rand.NextVector2(0, 0.3f);
                NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));

                NeonShooterGame.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                NeonShooterGame.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDead)
                base.Draw(spriteBatch);
        }

        public void Kill()
        {
            PlayerStatus.RemoveLife();
            _framesUntilRespawn = PlayerStatus.IsGameOver ? 300 : 120;

            Color explosionColor = new Color(0.8f, 0.8f, 0.4f); // yellow

            for (int i = 0; i < 1200; i++)
            {
                float speed = 18f * (1f - 1 / _rand.NextFloat(1f, 10f));
                Color color = Color.Lerp(Color.White, explosionColor, _rand.NextFloat(0, 1));
                var state = new ParticleState()
                {
                    Velocity = _rand.NextVector2(speed, speed),
                    Type = ParticleType.None,
                    LengthMultiplier = 1
                };

                NeonShooterGame.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
            }
        }

        public void Push(int force, Vector2 direction)
        {
            Velocity += force * direction.ToNormalized();
        }
    }
}