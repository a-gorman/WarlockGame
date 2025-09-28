using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;
using WarlockGame.Core.Game.Sim.Order;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities
{
    class Warlock : Entity {
        public float Speed { get; set; } = 3.5f;
        public const float RotationSpeed = 0.095f; // Radians per tick

        public float MaxHealth => 1;
        public float Health { get; set => field = float.Clamp(value, 0, MaxHealth); }

        /// Desired direction of travel. If set, the Warlock will move and rotate as needed
        public Vector2? Direction { get; set; }
        public float? DesiredOrientation { get; set; }

        public List<Buff> Buffs { get; } = new();

        public float DamageMultiplier { get; set; }= 1;
        
        public event Action<Warlock>? Respawned;
        public event Action<Warlock>? Destroyed;
        public event Action<Warlock>? SpellCast;

        private static readonly Random _rand = new();
        private readonly Simulation _sim;
        private int _nextBuffId = 1;

        private LinkedList<IOrder> Orders { get; } = new();


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
            
            foreach (var buff in Buffs) {
                buff.Update(this);
                if(buff.IsExpired) { buff.OnRemove(this); }
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
                var targetOrientation = Direction.Value.ToAngle();
                var interiorAngle = Util.Geometry.GetInteriorAngle(targetOrientation, Orientation);
                var rotationFactor = MathF.Cos(interiorAngle / 2).Squared();
                Velocity += Speed * rotationFactor * Direction.Value;

                if (Math.Abs(interiorAngle) < RotationSpeed) {
                    Orientation = targetOrientation;
                }
                else {
                    Orientation -= Math.Sign(interiorAngle) * RotationSpeed;
                }
            } else if(DesiredOrientation != null) {
                var interiorAngle = Util.Geometry.GetInteriorAngle(DesiredOrientation.Value, Orientation);

                if (Math.Abs(interiorAngle) < RotationSpeed) {
                    Orientation = DesiredOrientation.Value;
                }
                else {
                    Orientation -= Math.Sign(interiorAngle) * RotationSpeed;
                }
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, Sprite.Size / 2, Simulation.ArenaSize - Sprite.Size / 2);
        }
        
        public void CastSpell(int spellId, Vector2? castDirection) {
            if (_sim.SpellManager.Spells.TryGetValue(spellId, out var spell) && !spell.OnCooldown) {
                spell.DoCast(this, castDirection);
                SpellCast?.Invoke(this);
            }
        }

        public int AddBuff(Buff buff) {
            buff.Id = _nextBuffId++;
            Buffs.Add(buff);
            buff.OnAdd(this);
            return buff.Id;
        }
        
        public void RemoveBuff(int buffId) {
            foreach (var buff in Buffs) {
                if (buffId == buff.Id) {
                    buff.IsExpired = true;
                    return;
                }
            }
        }
        
        public override void Damage(float damage, Entity source) {
            Health -= damage;
            base.Damage(damage, source);

            if (Health <= 0) {
                Destroy(source);
            }
        }

        public void Respawn() {
            Health = MaxHealth;
            IsDead = false;
            Velocity = Vector2.Zero;
            Respawned?.Invoke(this);
        }
        
        private void Destroy(Entity source) {
            if (IsDead) return;
            
            Logger.Info($"Warlock {Id} destroyed! Source: {source.Id} Force: {source.PlayerId}");
            IsDead = true;
            foreach (var buff in Buffs) {
                if (buff.ClearedOnDeath) {
                    buff.IsExpired = true;
                }
            }
            CancelOrders();
            Destroyed?.Invoke(this);
        }
    }
}