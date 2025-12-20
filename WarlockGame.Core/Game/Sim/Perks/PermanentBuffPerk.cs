using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using WarlockGame.Core.Game.Log;
using WarlockGame.Core.Game.Sim.Buffs;

namespace WarlockGame.Core.Game.Sim.Perks;

abstract class PermanentBuffPerk : Perk {
    public PermanentBuffPerk(int id, string name, string description, Texture2D texture) 
        : base(id, name, description, texture) { }

    private readonly Dictionary<int, int> _playerBuffIds = new();

    protected abstract Buff CreateBuff();

    public override void OnAdded(int forceId, Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(forceId);
        if (warlock == null) {
            Logger.Warning($"Could not add buff because warlock does not exist. TypeId: {Id}. ForceId: {forceId}", Logger.LogType.Simulation);
            return;
        }

        var buff = CreateBuff();
        buff.ClearedOnDeath = false;
        buff.Timer = null;
        int buffId = warlock.AddBuff(buff);
        if (_playerBuffIds.TryAdd(forceId, buffId)) {
            Logger.Debug($"Added permanent buff. Type: {Id}. ForceId: {forceId}. BuffId: {buffId}", Logger.LogType.Simulation);
        }
        else {
            Logger.Warning($"Buff already exists. Could not add permanent buff. Type: {Id}. ForceId: {forceId}.", Logger.LogType.Simulation);
        }
    }

    public override void OnRemoved(int forceId, Simulation sim) {
        var warlock = sim.EntityManager.GetWarlockLivingOrDeadByForceId(forceId);
        if (warlock != null && _playerBuffIds.TryGetValue(forceId, out var buffId)) {
            warlock.RemoveBuff(buffId);
            Logger.Debug($"Removed permanent buff. Type: {Id}. ForceId: {forceId}", Logger.LogType.Simulation);
        }
        else {
            Logger.Warning($"Could not remove buff because warlock does not exist, or buff does not exist. Type: {Id}. ForceId: {forceId}", Logger.LogType.Simulation);
        }
    }
}