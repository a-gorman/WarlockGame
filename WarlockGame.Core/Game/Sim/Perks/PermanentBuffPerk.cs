using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

abstract class PermanentBuffPerk : Perk {
    private int _buffId;
    
    public PermanentBuffPerk(int forceId, PerkType type) : base(forceId, type) { }

    protected abstract Buff CreateBuff();

    public override void OnAdd(Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(ForceId);
        if (warlock != null) {
            var buff = CreateBuff();
            buff.ClearedOnDeath = false;
            buff.Timer = null;
            _buffId = warlock.AddBuff(buff);
        }
        else {
            Logger.Warning($"Could not add invisibility buff because warlock does not exist. ForceId: {ForceId}");
        }
    }

    public override void OnRemoved(Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(ForceId);
        if (warlock != null) {
            warlock.RemoveBuff(_buffId);
        }
        else {
            Logger.Warning($"Could not remove invisibility buff because warlock does not exist. ForceId: {ForceId}");
        }
    }
}