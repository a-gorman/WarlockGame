using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Sim.Buff;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;
using WarlockGame.Core.Game.Sim.Order;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities
{
    class Warlock : Entity
    {
        private readonly Simulation _sim;
        public const float Speed = 4;
        public const float RotationSpeed = 0.1f; // Radians per tick

        public float MaxHealth { get; private set; } = 60;
        public float Health { get; set; }

        public Vector2? Direction { get; set; }

        public List<IBuff> Buffs { get; } = new();
        
        private int _framesUntilRespawn = 0;
        public bool IsDead => _framesUntilRespawn > 0;

        private static readonly Random _rand = new();
        private LinkedList<IOrder> Orders { get; } = new();
        public event Action<Warlock>? Destroyed;

        public Warlock(int playerId, Vector2 position, Simulation simulation):
            base(new Sprite(Art.Player), position, radius: 20) {
            _sim = simulation;
            Health = MaxHealth;
            PlayerId = playerId;
            BlocksProjectiles = true;
            
            AddBehaviors(new Pushable());
        }

        public override void Update() {
            Orders.FirstOrDefault()?.Update();

            Move();
            
            MakeExhaustFire();

            foreach (var buff in Buffs) {
                buff.Update(this);
            }
            Buffs.RemoveAll(x => x.IsExpired);

            if (Orders.FirstOrDefault()?.Finished ?? false) {
                Orders.First!.Value.OnFinish();
                Orders.RemoveFirst();
            }
            
            base.Update();
        }

        public void GiveOrder(Func<Warlock, IOrder> order) {
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
            if (Velocity.LengthSquared() < Speed.Squared()) {
                Velocity = Vector2.Zero;
            }
            else {
                Velocity -= Velocity.WithLength(Speed);
            }

            if (Direction != null) {
                var targetOrientation = Extensions.ToAngle(Direction.Value);
                var interiorAngle = Util.Geometry.GetInteriorAngle(targetOrientation, Orientation);
                var rotationFactor = MathF.Cos(interiorAngle / 2).Squared();
                Velocity += Speed * rotationFactor * Direction.Value;

                if (Math.Abs(interiorAngle) < RotationSpeed) {
                    Orientation = targetOrientation;
                }
                else {
                    Orientation += -Math.Sign(interiorAngle) * RotationSpeed;
                }
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, Sprite.Size / 2, Simulation.ArenaSize - Sprite.Size / 2);
        }
        
        public void CastSpell(int spellId, Vector2 castDirection) {
            if(_sim.SpellManager.Spells.TryGetValue(spellId, out var spell) && !spell.OnCooldown) {
                spell.DoCast(this, castDirection);
            }
        }

        public void AddBuff(IBuff buff) {
            Buffs.Add(buff);
        }

        // Also secretly rotates the ship
        private void MakeExhaustFire()
        {
            if (Velocity.LengthSquared() > 0.1f)
            {
                // set up some variables
                Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, Orientation);

                double t = WarlockGame.GameTime.TotalGameTime.TotalSeconds;
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
                WarlockGame.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));
                WarlockGame.ParticleManager.CreateParticle(Art.Glow, pos, midColor * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(velMid, ParticleType.Enemy));

                // side particle streams
                Vector2 vel1 = baseVel + perpVel + _rand.NextVector2(0, 0.3f);
                Vector2 vel2 = baseVel - perpVel + _rand.NextVector2(0, 0.3f);
                WarlockGame.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                WarlockGame.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));

                WarlockGame.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(vel1, ParticleType.Enemy));
                WarlockGame.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f,
                    new Vector2(0.5f, 1),
                    new ParticleState(vel2, ParticleType.Enemy));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDead)
                base.Draw(spriteBatch);
        }

        public override void Damage(float damage, Entity source) {
            Health -= damage;
            base.Damage(damage, source);

            if (Health <= 0) {
                Destroy(source);
            }
        }

        private void Destroy(Entity source) {
            if (!IsExpired) {
                IsExpired = true;
                Destroyed?.Invoke(this);
            }
        }
    }
}