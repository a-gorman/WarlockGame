using System;
using System.Collections.Generic;
using System.Linq;
using WarlockGame.Core.Game.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Buffs;
using WarlockGame.Core.Game.Sim.Entities.Behaviors;
using WarlockGame.Core.Game.Sim.Order;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.Sim.Entities;

class Warlock : Entity {
    public float Speed { get; set; } = 3.0f;
    public const float RotationSpeed = 0.15f; // Radians per tick

    public float MaxHealth => 60;
    public float Health { get; set => field = float.Clamp(value, 0, MaxHealth); }

    /// Desired direction of travel. If set, the Warlock will move and rotate as needed
    public Vector2? Direction {
        get;
        set {
            if (value.HasValue ^ field.HasValue) {
                field = value;
                RecalculateMoveState();
            }
        }
    }

    public float? DesiredOrientation {
        get;
        set {
            if (value.HasValue ^ field.HasValue) {
                field = value;
                RecalculateMoveState();
            }
        }
    }

    private bool _updatingBuffs = false;
    private List<Buff> _addedBuffs = [];
    public List<Buff> Buffs { get; } = new();

    public float DamageMultiplier { get; set; } = 1;

    public float GenericDefense { get; set; } = 1;
    public float PlayerDefense { get; set; } = 1;
    public float EnvironmentDefense { get; set; } = 1;
    public float BoundsDefense { get; set; } = 1;
    
    private bool Sliding { get;
        set {
            if (value != field) {
                field = value;
                RecalculateMoveState();
            }
        }
    }
    private MoveState _moveState;
    private readonly Friction _slidingFriction;
        
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

        _slidingFriction = new Friction(a: 0.001f, b: 0.12f, c: 0.02f);
        
        OnPushed += HandlePushed;
        AddBehaviors(new Pushable());
    }

    public override void Update() {
        Orders.FirstOrDefault()?.Update();

        Move();

        _updatingBuffs = true;
        foreach (var buff in Buffs) {
            buff.Update(this);
            if(buff.IsExpired) { buff.OnRemove(this); }
        }
        Buffs.RemoveAll(x => x.IsExpired);
        _updatingBuffs = false;
        foreach (var newBuff in _addedBuffs) {
            AddBuff(newBuff);
        }
        _addedBuffs.Clear();
        
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
        switch (_moveState) {
            case MoveState.Sliding: {
                if (Velocity == Vector2.Zero) {
                    RemoveBehaviors(_slidingFriction);
                    Sliding = false;
                }
                break;
            }
            case MoveState.Moving when Direction != null: {
                var targetOrientation = Direction.Value.ToAngle();
                var interiorAngle = Util.Geometry.GetInteriorAngle(targetOrientation, Orientation);
                var rotationFactor = MathF.Cos(interiorAngle / 2).Squared();
                Velocity = Speed * rotationFactor * Direction.Value;

                if (Math.Abs(interiorAngle) < RotationSpeed) {
                    Orientation = targetOrientation;
                }
                else {
                    Orientation -= Math.Sign(interiorAngle) * RotationSpeed;
                }
                break;
            }
            case MoveState.Rotating: {
                if (DesiredOrientation != null) {
                    var interiorAngle = Util.Geometry.GetInteriorAngle(DesiredOrientation.Value, Orientation);

                    if (Math.Abs(interiorAngle) < RotationSpeed) {
                        Orientation = DesiredOrientation.Value;
                    }
                    else {
                        Orientation -= Math.Sign(interiorAngle) * RotationSpeed;
                    }
                }
                break;
            }
            case MoveState.Stopped:
                Velocity = Vector2.Zero;
                break;
            default:
                Logger.Error($"Invalid move state value {(int)_moveState}", Logger.LogType.Simulation);
                RecalculateMoveState();
                break;
        }

        Position += Velocity;
        Position = Vector2.Clamp(Position, Sprite.Size / 2, Simulation.ArenaSize - Sprite.Size / 2);
    }
        
    public void CastSpell(int spellId, Vector2 castTarget) {
        if (_sim.SpellManager.Spells.TryGetValue(spellId, out var spell) && !spell.OnCooldown) {
            spell.DoCast(this, castTarget);
            SpellCast?.Invoke(this);
        }
    }

    public int AddBuff(Buff buff) {
        if (_updatingBuffs) {
            buff.Id = _nextBuffId++;
            _addedBuffs.Add(buff);
        }
        
        var existingBuff = Buffs.FirstOrDefault(x => x.Type == buff.Type);
        if (existingBuff == null || buff.Stacking == Buff.StackingType.Stacks) {
            buff.Id = _nextBuffId++;
            Buffs.Add(buff);
            buff.OnAdd(this);
        } else if (buff.Stacking == Buff.StackingType.Refreshes && existingBuff.Timer != null) {
            Logger.Debug($"Refreshing {existingBuff.Type} buff {existingBuff.Id}", Logger.LogType.Simulation);
            existingBuff.Timer = buff.Timer?.Let(x =>
                GameTimer.FromTicks(Math.Max(x.TicksRemaining, existingBuff.Timer!.Value.TicksRemaining)));
        }

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
        
    public override void Damage(float damage, DamageType damageTypes, Entity? source) {
        var effectiveDamage = damage * GenericDefense;
        if (damageTypes.HasType(DamageType.Player)) {
            effectiveDamage *= PlayerDefense;
        }
        if (damageTypes.HasType(DamageType.Bounds)) {
            effectiveDamage *= BoundsDefense;
        }
        if (damageTypes.HasType(DamageType.Environment)) {
            effectiveDamage *= EnvironmentDefense;
        }
        Health -= effectiveDamage;
        base.Damage(damage, damageTypes, source);

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
        
    public void Destroy(Entity? source) {
        if (IsDead) return;
            
        Logger.Info($"Warlock {Id} destroyed! Source: {source?.Id} Force: {source?.PlayerId}", Logger.LogType.Simulation);
        IsDead = true;
        foreach (var buff in Buffs) {
            if (buff.ClearedOnDeath) {
                buff.IsExpired = true;
            }
        }
        CancelOrders();
        Destroyed?.Invoke(this);
    }

    private void HandlePushed(OnPushedEventArgs args) {
        Sliding = true;
        AddBehaviors(_slidingFriction);
    }

    private void RecalculateMoveState() {
        if (Sliding) {
            _moveState = MoveState.Sliding;
        } else if (Direction != null) {
            _moveState = MoveState.Moving;
        } else if(DesiredOrientation != null) {
            _moveState = MoveState.Rotating;
        } else {
            _moveState = MoveState.Stopped;
        }
    }
}

internal enum MoveState {
    Stopped,
    Rotating,
    Moving,
    Sliding
}