namespace WarlockGame.Core.Game.Sim.Buffs.Behaviors;

closed class BuffComponent {

    public virtual void OnUpdate(Buff buff, Warlock target) { }

    public virtual void OnAdd(Buff buff, Warlock target) { }

    public virtual void OnRemove(Buff buff, Warlock target) { }

    public virtual void OnRespawn(Buff buff) { }
}