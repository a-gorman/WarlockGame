using System.Collections.Generic;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

abstract class PermanentBuffPerk : Perk {
    public PermanentBuffPerk(PerkType type) : base(type) { }

    private readonly Dictionary<int, int> _playerBuffIds = new();

    protected abstract Buff CreateBuff();

    public override void OnChosen(int forceId, Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(forceId);
        if (warlock == null) {
            Logger.Warning($"Could not add buff because warlock does not exist. Type: {Type}. ForceId: {forceId}");
            return;
        }

        var buff = CreateBuff();
        buff.ClearedOnDeath = false;
        buff.Timer = null;
        int buffId = warlock.AddBuff(buff);
        if (_playerBuffIds.TryAdd(forceId, buffId)) {
            Logger.Debug($"Added permanent buff. Type: {Type}. ForceId: {forceId}. BuffId: {buffId}");
        }
        else {
            Logger.Warning($"Buff already exists. Could not add permanent buff. Type: {Type}. ForceId: ${forceId}.");
        }
    }

    public override void OnPerkRemoved(int forceId, Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(forceId);
        if (warlock != null && _playerBuffIds.TryGetValue(forceId, out var buffId)) {
            warlock.RemoveBuff(buffId);
            Logger.Debug($"Removed permanent buff. Type: {Type}. ForceId: {forceId}");
        }
        else {
            Logger.Warning($"Could not remove buff because warlock does not exist, or buff does not exist. Type: {Type}. ForceId: {forceId}");
        }
    }
}