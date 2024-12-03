using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Buff;
using WarlockGame.Core.Game.Util;
using WarlockGame.Core.Game.Entity.Order;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim;
using WarlockGame.Core.Game.Spell;

namespace WarlockGame.Core.Game.Entity
{
    class Warlock : EntityBase
    {
        public const float Speed = 4;

        public float MaxHealth { get; private set; } = 100;

        public float Health { get; set; }

        public event Action<Warlock>? Destroyed;
        
        public Vector2? Direction { get; set; }

        public List<WarlockSpell> Spells { get; } = new() { SpellFactory.Fireball(), SpellFactory.Lightning(), SpellFactory.Poison(), SpellFactory.Burst(), SpellFactory.WindShield() };
        public List<IBuff> Buffs { get; } = new();
        
        public int PlayerId { get; }

        private int _framesUntilRespawn = 0;
        public bool IsDead => _framesUntilRespawn > 0;

        private static readonly Random _rand = new();

        private LinkedList<IOrder> Orders { get; } = new();

        public Warlock(int playerId) :
            base(new Sprite(Art.Player)) {
            
            Health = MaxHealth;
            PlayerId = playerId;
            Position = Simulation.ArenaSize / 2;
            Radius = 20;
        }

        public override void Update() {
            Orders.FirstOrDefault()?.Update();

            Move();
            
            // Debug.Visualize(Position.ToString(), new Vector2(100,100));
            
            MakeExhaustFire();

            foreach (var spell in Spells) {
                spell.Update();
            }

            foreach (var buff in Buffs) {
                buff.Update(this);
            }
            Buffs.RemoveAll(x => x.IsExpired);

            if (Orders.FirstOrDefault()?.Finished ?? false) {
                Orders.First!.Value.OnFinish();
                Orders.RemoveFirst();
            }
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
            Position = Vector2.Clamp(Position, _sprite.Size / 2, Simulation.ArenaSize - _sprite.Size / 2);

            if (Velocity.HasLength())
                Orientation = Velocity.ToAngle();
        }
        
        public void CastSpell(int spellId, Vector2 castDirection) {
            var spell = Spells.Find(x => spellId == x.SpellId);
            
            if (spell is not null && !spell.OnCooldown)
            {
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

        public void Push(float force, Vector2 direction)
        {
            Velocity += force * direction.ToNormalizedOrZero();
        }

        public void Damage(float damage, IEntity source) {
            Health -= damage;

            if (Health <= 0) {
                Destroy(source);
            }
        }

        private void Destroy(IEntity source) {
            if (!IsExpired) {
                IsExpired = true;
                Destroyed?.Invoke(this);
            }
        }
    }
}