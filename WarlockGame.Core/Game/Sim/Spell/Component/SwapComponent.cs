
class SwapComponent: IEntityComponent {
    public void Invoke(SpellContext context, IReadOnlyCollection<TargetInfo> targets) {
        if(targets.isEmpty()) return;

        var casterPos = context.Caster.Position;
        context.Caster.Position = targets.first().entity.Position - targets.first().OriginTargetDisplacement;
        foreach(var target in targets) {
            target.Entity.Position = casterPos + target.OriginTargetDisplacement;
        }
    }
}