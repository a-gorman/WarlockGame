//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeonShooter.Core.Game.Entity.Order;
using NeonShooter.Core.Game.Graphics;
using NeonShooter.Core.Game.Spell;
using NeonShooter.Core.Game.Util;
using NeonShooter.Core.Game.UX;

namespace NeonShooter.Core.Game.Entity
{
    class PlayerShip : EntityBase
    {
        public const float Speed = 8;

        public const float MaxHealth = 100;

        public float Health { get; private set; } = MaxHealth;

        public event Action<PlayerShip> Destroyed;
        
        public Vector2? Direction { get; set; }

        public List<WarlockSpell> Spells { get; } = new() { SpellFactory.Fireball(), SpellFactory.Lightning() };
        
        public Player Player { get; }

        private int _framesUntilRespawn = 0;
        public bool IsDead => _framesUntilRespawn > 0;

        private static readonly Random _rand = new();

        private LinkedList<IOrder> Orders { get; } = new();

        public PlayerShip(Player player) :
            base(new Sprite(Art.Player))
        {
            Player = player;
            Position = NeonShooterGame.ScreenSize / 2;
            Radius = 10;
        }

        public override void Update() {
            // if (IsDead) {
            //     if (--_framesUntilRespawn == 0) {
            //         if (Player.Status.Lives == 0) {
            //             Player.Status.Reset();
            //             Position = NeonShooterGame.ScreenSize / 2;
            //         }
            //         NeonShooterGame.Grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(Position, 0), 50);
            //     }
            //     return;
            // }

            Orders.FirstOrDefault()?.Update();

            Move();
            
            Debug.Visualize(Position.ToString(), new Vector2(100,100));
            
            MakeExhaustFire();

            foreach (var spell in Spells) {
                spell.Update();
            }

            if (Orders.FirstOrDefault()?.Finished ?? false) {
                Orders.First!.Value.OnFinish();
                Orders.RemoveFirst();
            }
        }

        public void CastSpell(int spellIndex, Vector2 castDirection) {
            if(spellIndex < Spells.Count)
                CastSpell(Spells[spellIndex], castDirection);
        }

        public void GiveOrder(Func<PlayerShip, IOrder> order) {
            GiveOrder(order(this));
        }
        
        private void GiveOrder(IOrder order) {
            CancelOrders();
            Orders.AddFirst(order);
        }
        
        private void CancelOrders() {
            while (Orders.Count != 0) {
                Orders.First?.Value.OnCancel();
                Orders.RemoveFirst();
            }
        }

        private void Move() {
            if (Velocity.IsLengthLessThan(Speed)) {
                Velocity = Vector2.Zero;
            }
            else {
                Velocity -= Velocity.WithLength(Speed);
            }

            if (Direction != null) {
                Velocity += Speed * (Vector2)Direction;
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, _sprite.Size / 2, NeonShooterGame.ScreenSize - _sprite.Size / 2);

            if (Velocity.HasLength())
                Orientation = Velocity.ToAngle();
        }

        public void CastSpell(WarlockSpell spell, Vector2 castDirection)
        {
            if (!spell.OnCooldown && castDirection.HasLength())
            {
                spell.Cast(this, castDirection);
            }
        }

        // Also secretly rotates the ship
        private void MakeExhaustFire()
        {
            if (Velocity.LengthSquared() > 0.1f)
            {
                // set up some variables
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
                Debug.Visualize(Health.ToString(), Position);
        }

        public void Kill()
        {
            Player.Status.RemoveLife();
            _framesUntilRespawn = GameStatus.IsGameOver ? 300 : 120;

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

        public void Damage(float damage, IEntity source) {
            Health -= damage;

            if (Health <= 0) {
                Destroy(source);
            }
        }

        private void Destroy(IEntity source) {
            IsExpired = true;
            // source.
        }
    }
}