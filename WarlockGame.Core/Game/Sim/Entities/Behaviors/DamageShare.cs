namespace WarlockGame.Core.Game.Sim.Entities.Behaviors;

class DamageShare : Behavior {
    private readonly int _targetId;
    private readonly Simulation _simulation;

    public DamageShare(int targetId, Simulation simulation) {
        _targetId = targetId;
        _simulation = simulation;
    }
    
    public override void OnAdd(Entity entity) {
        entity.OnDamaged += ShareDamage;
    }
    
    public override void OnRemove(Entity entity) {
        entity.OnDamaged -= ShareDamage;
    }

    private void ShareDamage(OnDamagedEventArgs args) {
        var target = _simulation.EntityManager.GetEntity(_targetId);
        if (target is not null) {
            if (!args.DamageTypes.HasType(DamageType.Shared)) {
                target.Damage(args.Amount, args.DamageTypes | DamageType.Shared, args.DamageSource);
            }
        }
        else {
            IsExpired = true;
        }
    }
}